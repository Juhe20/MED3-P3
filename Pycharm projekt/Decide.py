import cv2
import numpy as np
import socket
import json

#Connect to the local server
host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

#Load image
img = cv2.imread("Images/Makvaer_Test.jpg")
img = cv2.resize(img, (600, 800))

#Grabcut for background removal
mask = np.zeros(img.shape[:2], np.uint8)
rect = (180, 180, img.shape[1] - 300, img.shape[0] - 360)
background = np.zeros((1, 65), np.float64)
foreground = np.zeros((1, 65), np.float64)
cv2.grabCut(img, mask, rect, background, foreground, 10, cv2.GC_INIT_WITH_RECT)

#Make background transparent and extract foreground into new array
mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')
img_foreground = img * mask2[:, :, np.newaxis]
img_transparent = cv2.cvtColor(img_foreground, cv2.COLOR_BGR2BGRA)
img_transparent[:, :, 3] = mask2 * 255

#Board detection
img_gray = cv2.cvtColor(img_foreground, cv2.COLOR_BGR2GRAY)
blurred = cv2.GaussianBlur(img_gray, (7, 7), 0)
edges = cv2.Canny(blurred, 50, 150)
kernel = np.ones((5, 5), np.uint8)
edges_cleaned = cv2.morphologyEx(edges, cv2.MORPH_CLOSE, kernel)
contours, _ = cv2.findContours(edges_cleaned, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

#Check if any contours were found
if len(contours) > 0:
    largest_contour = max(contours, key=cv2.contourArea)
    epsilon = 0.01 * cv2.arcLength(largest_contour, True)
    approx = cv2.approxPolyDP(largest_contour, epsilon, True)

    #Aspect ratio and solidity for board shape detection
    x, y, w, h = cv2.boundingRect(largest_contour)
    aspect_ratio = float(w) / h
    hull = cv2.convexHull(largest_contour)
    hull_area = cv2.contourArea(hull)
    contour_area = cv2.contourArea(largest_contour)
    solidity = contour_area / hull_area

    board_area = cv2.contourArea(largest_contour)
    cv2.drawContours(img, [approx], -1, (0, 255, 0), 3)

    #Perspective transform to fix board distortion
    bottomLeft = [x, y + h]
    bottomRight = [x + w, y + h]
    topRight = [x + w, y]
    topLeft = [x, y]
    widthLeft = np.sqrt(((bottomLeft[0] - topLeft[0]) ** 2) + ((bottomLeft[1] - topLeft[1]) ** 2))
    widthRight = np.sqrt(((bottomRight[0] - topRight[0]) ** 2) + ((bottomRight[1] - topRight[1]) ** 2))
    maxWidth = max(int(widthLeft), int(widthRight))
    heightBottom = np.sqrt(((bottomLeft[0] - bottomRight[0]) ** 2) + ((bottomLeft[1] - bottomRight[1]) ** 2))
    heightTop = np.sqrt(((topRight[0] - topLeft[0]) ** 2) + ((topRight[1] - topLeft[1]) ** 2))
    maxHeight = max(int(heightBottom), int(heightTop))

    input_pts = np.float32([bottomLeft, bottomRight, topRight, topLeft])
    output_pts = np.float32([[0, maxHeight], [maxWidth, maxHeight], [maxWidth, 0], [0, 0]])
    transformPosition = cv2.getPerspectiveTransform(input_pts, output_pts)
    out = cv2.warpPerspective(img_gray, transformPosition, (maxWidth, maxHeight), flags=cv2.INTER_LINEAR)

    #Hough Circle Transform for piece detection
    detected_circles = cv2.HoughCircles(out, cv2.HOUGH_GRADIENT, 0.2, 20, param1=45, param2=12.7, minRadius=12, maxRadius=17)
    positiondata = {}
    white = 0
    black = 0
    circles = 0
    grid_size = 8
    grid = np.zeros((grid_size, grid_size))
    tile_width = maxWidth // grid_size
    tile_height = maxHeight // grid_size
    #Loop through each detected circles coordinates to add to grid
    if detected_circles is not None:
        detected_circles = np.uint16(np.around(detected_circles))
        for coord in detected_circles[0, :]:
            x, y, r = coord[0], coord[1], coord[2]
            circles += 1
            col = x // tile_width
            row = y // tile_height
            if grid[row, col] == 0:
                grid[row, col] = 1
            cv2.circle(out, (x, y), r, (0, 255, 0), 2)
            cv2.circle(out, (x, y), 1, (0, 0, 255), 3)
            #Use LAB values to get average brightness on the pieces
            img_LAB = cv2.cvtColor(img, cv2.COLOR_BGR2LAB)
            L, A, B = cv2.split(img_LAB)
            roi_size = 4
            x1, y1 = max(0, x - roi_size), max(0, y - roi_size)
            x2, y2 = min(B.shape[1], x + roi_size), min(B.shape[0], y + roi_size)
            roi = B[y1:y2, x1:x2]
            mean_B = np.mean(roi)
            #Check mean value of B in LAB
            if mean_B > 131:
                black += 1
                positiondata[f"black{black}"] = [int(col), 0, int(row)]
            else:
                white += 1
                positiondata[f"white{white}"] = [int(col), 0, int(row)]

    #Check solidity and aspect ratio to find the game
    if solidity > 0.85 < aspect_ratio:
        detected_game = "Makvaer"
    elif solidity < 0.4 or (solidity < 0.6 and aspect_ratio > 0.7):
        detected_game = "Gaasetavl"
    else:
        detected_game = "HundEfterHare"
        #detected_game = "Makvaer"

    print(f"Number of circles detected: {circles}")
    print(f"Position data: {positiondata}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")
    print(f"What game is it: {detected_game}")

    #Send data to local server
    positions = json.dumps(positiondata)
    sock.sendall(positions.encode("UTF-8"))
    sock.sendall(detected_game.encode("UTF-8"))
    receivedData = sock.recv(1024).decode("UTF-8")
    print(receivedData)
