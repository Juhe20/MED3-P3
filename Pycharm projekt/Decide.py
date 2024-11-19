import cv2
import numpy as np
import socket
import json

# Connect to the server
host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

# def divideStandardImageIntoSections(image, h_size, w_size, ignore_tiles=None):  # Method for standard grid division
#     if ignore_tiles is None:
#         ignore_tiles = []
#
#     height, width = image.shape
#     tile_height = height // h_size
#     tile_width = width // w_size
#
#     tile_positions = []  # Empty list to store the positions (& images for each tile)
#     for ih in range(h_size):
#         for iw in range(w_size):
#             if (ih, iw) in ignore_tiles:
#                 continue
#             x = iw * tile_width
#             y = ih * tile_height
#             tile_img = image[y:y + tile_height, x:x + tile_width]
#             tile_positions.append(((ih, iw), tile_img))
#
#             cv2.waitKey(0)
#             cv2.destroyAllWindows()
#             tile_positions.append((ih, iw))
#
#     detected_circles = cv2.HoughCircles(image, cv2.HOUGH_GRADIENT, 0.2, 20, param1=55, param2=40, minRadius=1,
#                                         maxRadius=30)
#     print(detected_circles)
#     return tile_positions  # Returns list of tile positions (and for now images)
#
#
# def divideSpecialImageIntoSections(image, row_lengths, ignore_tiles=None):  # Custom column per row division
#     if ignore_tiles is None:
#         ignore_tiles = []
#     height, width = image.shape
#     tile_height = height // len(row_lengths)
#
#     tile_positions = []
#     for ih, row_length in enumerate(row_lengths):
#         tile_width = width // row_length
#         for iw in range(row_length):
#             if (ih, iw) in ignore_tiles:
#                 continue
#             x = iw * tile_width + 10
#             y = ih * tile_height + 10
#             tile_img = image[y:y + tile_height, x:x + tile_width]
#             tile_positions.append(((ih, iw), tile_img))
#     return tile_positions


# Load image
img = cv2.imread("Makvaer/IMG_0827.jpg")
img = cv2.resize(img, (600, 800))

# Grabcut for background removal
mask = np.zeros(img.shape[:2], np.uint8)
rect = (50, 50, img.shape[1] - 100, img.shape[0] - 100)
background = np.zeros((1, 65), np.float64)
foreground = np.zeros((1, 65), np.float64)
cv2.grabCut(img, mask, rect, background, foreground, 5, cv2.GC_INIT_WITH_RECT)

mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')
img_foreground = img * mask2[:, :, np.newaxis]
img_transparent = cv2.cvtColor(img_foreground, cv2.COLOR_BGR2BGRA)
img_transparent[:, :, 3] = mask2 * 255

# Board and piece detection
img_gray = cv2.cvtColor(img_foreground, cv2.COLOR_BGR2GRAY)
blurred = cv2.GaussianBlur(img_gray, (5, 5), 0)
edges = cv2.Canny(blurred, 50, 150)
kernel = np.ones((5, 5), np.uint8)
edges_cleaned = cv2.morphologyEx(edges, cv2.MORPH_CLOSE, kernel)
contours, _ = cv2.findContours(edges_cleaned, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Check if any contours were found
if len(contours) > 0:
    largest_contour = max(contours, key=cv2.contourArea)
    x_coords = largest_contour.reshape(-1, 2)[:, 0]
    y_coords = largest_contour.reshape(-1, 2)[:, 1]
    epsilon = 0.02 * cv2.arcLength(largest_contour, True)
    approx = cv2.approxPolyDP(largest_contour, epsilon, True)

    # Aspect ratio and solidity for board shape detection
    x, y, w, h = cv2.boundingRect(largest_contour)
    aspect_ratio = float(w) / h
    hull = cv2.convexHull(largest_contour)
    hull_area = cv2.contourArea(hull)
    contour_area = cv2.contourArea(largest_contour)
    solidity = contour_area / hull_area

    board_area = cv2.contourArea(largest_contour)
    cv2.drawContours(img, [approx], -1, (0, 255, 0), 3)

    # Perspective transform to fix board distortion
    pt_A = [x, y + h]
    pt_B = [x + w, y + h]
    pt_C = [x + w, y]
    pt_D = [x, y]
    width_AD = np.sqrt(((pt_A[0] - pt_D[0]) ** 2) + ((pt_A[1] - pt_D[1]) ** 2))
    width_BC = np.sqrt(((pt_B[0] - pt_C[0]) ** 2) + ((pt_B[1] - pt_C[1]) ** 2))
    maxWidth = max(int(width_AD), int(width_BC))
    height_AB = np.sqrt(((pt_A[0] - pt_B[0]) ** 2) + ((pt_A[1] - pt_B[1]) ** 2))
    height_CD = np.sqrt(((pt_C[0] - pt_D[0]) ** 2) + ((pt_C[1] - pt_D[1]) ** 2))
    maxHeight = max(int(height_AB), int(height_CD))

    input_pts = np.float32([pt_A, pt_B, pt_C, pt_D])
    output_pts = np.float32([[0, maxHeight], [maxWidth, maxHeight], [maxWidth, 0], [0, 0]])
    M = cv2.getPerspectiveTransform(input_pts, output_pts)
    out = cv2.warpPerspective(img_gray, M, (maxWidth, maxHeight), flags=cv2.INTER_LINEAR)
    out_col = cv2.warpPerspective(img, M, (maxWidth, maxHeight), flags=cv2.INTER_LINEAR)

    # Hough Circle Transform for piece detection (after detecting the board)
    # Hough Circle Transform for piece detection
    detected_circles = cv2.HoughCircles(out, cv2.HOUGH_GRADIENT, 0.2, 20, param1=45, param2=20.9, minRadius=12,
                                        maxRadius=20)

    positiondata = {}
    white = 0
    black = 0
    circles = 0
    grid_size = 8
    grid = np.zeros((grid_size, grid_size))
    tile_width = maxWidth // grid_size
    tile_height = maxHeight // grid_size

    if detected_circles is not None:
        detected_circles = np.uint16(np.around(detected_circles))
        for pt in detected_circles[0, :]:
            a, b, r = pt[0], pt[1], pt[2]
            circles += 1
            col = a // tile_width
            row = b // tile_height

            # Check if the cell is already occupied
            if grid[row, col] == 0:
                # The cell is empty, so assign the piece to this cell
                grid[row, col] = 1  # Mark this cell as occupied

            cv2.circle(out, (a, b), r, (0, 255, 0), 2)
            cv2.circle(out, (a, b), 1, (0, 0, 255), 3)
            img_LAB = cv2.cvtColor(img, cv2.COLOR_BGR2LAB)
            L, A, B = cv2.split(img_LAB)
            roi_size = 4
            x1, y1 = max(0, a - roi_size), max(0, b - roi_size)
            x2, y2 = min(B.shape[1], a + roi_size), min(B.shape[0], b + roi_size)
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
    if solidity < 0.9:
        WhatGameIsIt = "Gaasetavl"
    elif solidity < 0.9 < aspect_ratio:
        WhatGameIsIt = "Makvaer"
    else:
        WhatGameIsIt = "HundEfterHare"

    print(f"Number of circles detected: {circles}")
    print(f"Position data: {positiondata}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")

    # Send data to server
    positions = json.dumps(positiondata)
    sock.sendall(positions.encode("UTF-8"))
    sock.sendall(WhatGameIsIt.encode("UTF-8"))
    receivedData = sock.recv(1024).decode("UTF-8")
    print(receivedData)

    # Display image
    cv2.imshow("Detected Board", out)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
