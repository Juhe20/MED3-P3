import cv2
import numpy as np
import matplotlib.pyplot as plt
import socket
import json


# Connect to the server
#host, port = "127.0.0.1", 25001
#sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#sock.connect((host, port))

def divideStandardImageIntoSections(image, h_size, w_size, ignore_tiles=None): #Method for the standard grid division
    #Optional list that includes the tiles that should be ignored. If ignore_tiles wasn't provided then sets it to an empty list
    if ignore_tiles is None:
        ignore_tiles = []

    height, width = image.shape
    #Calculating the height and width of each tile by dividing the image’s dimensions by the specified grid size
    tile_height = height // h_size
    tile_width = width // w_size

    tile_positions = [] #Empty list to store the positions (& if not removed images for each tile)
    for ih in range(h_size): #looping through the grid
        for iw in range(w_size):
            if (ih, iw) in ignore_tiles: #checking if the current position is in the inore tiles list, if yes we skip it
                continue
            # Purely for the display of the tiles alongside their position, remove later
            x = iw * tile_width
            y = ih * tile_height
            tile_img = image[y:y + tile_height, x:x + tile_width]
            tile_positions.append(((ih, iw), tile_img))

            # Display the tile
            cv2.imshow(f"Tile {ih},{iw}", tile_img)
            print(f"Tile at position {ih},{iw}")
            cv2.waitKey(0)
            cv2.destroyAllWindows()
            tile_positions.append((ih, iw))

    detected_circles = cv2.HoughCircles(image, cv2.HOUGH_GRADIENT, 0.2, 20, param1=55, param2=40, minRadius=1, maxRadius=30)
    print(detected_circles)
    return tile_positions #returns the list of tile positions (and for now images)

def divideSpecialImageIntoSections(image, row_lengths,  ignore_tiles=None): #Method for special division with custom number of columns per row
    if ignore_tiles is None:
        ignore_tiles = []
    height, width, _ = image.shape
    tile_height = height // len(row_lengths) #finding the hight of each row based on the number of rows

    tile_positions = [] #empty list to store the positions
    for ih, row_length in enumerate(row_lengths): #Iterates through row_lengths, where ih is the row index and row_length is the number of tiles in that row
        tile_width = width // row_length
        for iw in range(row_length):
            if (ih, iw) in ignore_tiles: #checking if the current position is in the inore tiles list, if yes we skip it
                continue
            x = iw * tile_width + 10
            y = ih * tile_height + 10
            # Purely for the display of the tiles alongside their position, remove later
            tile_img = image[y:y + tile_height, x:x + tile_width]
            tile_positions.append(((ih, iw), tile_img))

            # Display the tile
            cv2.imshow(f"Tile {ih},{iw}", tile_img)
            print(f"Tile at position {ih},{iw}")
            cv2.waitKey(0)  # Wait for key press to move to the next tile
            cv2.destroyAllWindows()
            tile_positions.append((ih, iw))
    return tile_positions

img1 = cv2.imread("FindingPositions/Makvaer.png")
img_gray1 = cv2.cvtColor(img1, cv2.COLOR_BGR2GRAY)
img2 = cv2.imread("FindingPositions/HundEfterHare.png")
img_gray2 = cv2.cvtColor(img2, cv2.COLOR_BGR2GRAY)
img3 = cv2.imread("FindingPositions/Gaasetavl.png")
img_gray3 = cv2.cvtColor(img3, cv2.COLOR_BGR2GRAY)

h_size_img1, w_size_img1 = 8, 8
h_size_img2, w_size_img2 = 9, 9
ignore_tiles_HundEfterHare = [
    (0, 0), (0, 1), (0, 2), (0, 3), (0, 5), (0, 6), (0, 7), (0, 8),
    (1, 0), (1, 2), (1, 3), (1, 5), (1, 6), (1, 8),
    (2, 0), (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8),
    (3, 0), (3, 1), (3, 2), (3, 3), (3, 4), (3, 5), (3, 6), (3, 7), (3, 8),
    (4, 2), (4, 3), (4, 5), (4, 6),
    (5, 0), (5, 1), (5, 2), (5, 3), (5, 4), (5, 5), (5, 6), (5, 7), (5, 8),
    (6, 0), (6, 1), (6, 2), (6, 3), (6, 4), (6, 5), (6, 6), (6, 7), (6, 8),
    (7, 0), (7, 2), (7, 3), (7, 5), (7, 6), (7, 8),
    (8, 0), (8, 1), (8, 2), (8, 3), (8, 5), (8, 6), (8, 7), (8, 8)
]
ignore_tiles_Gaasetavl = [
    (0, 0), (0, 1), (0, 5), (0, 6),
    (1, 0), (1, 4), (1, 5),
    (6, 0), (6, 1), (6, 5), (6, 6),
    (7, 0), (7, 5),
]

tile_positions_img1 = divideStandardImageIntoSections(img_gray1, h_size_img1, w_size_img1)
tile_positions_img2 = divideStandardImageIntoSections(img_gray2, h_size_img2, w_size_img2, ignore_tiles_HundEfterHare)

row_lengths_img3 = [7, 6, 7, 6, 7, 6, 7, 6]  #Defining the number of columns for each row
tile_positions_img3 = divideSpecialImageIntoSections(img_gray3, row_lengths_img3, ignore_tiles_Gaasetavl)

#Printing positions for each image
print("Positions Makvaer:")
for index, (row, col) in enumerate(tile_positions_img1):
    print(f"Tile {index}: Position {row},{col}")

print("\nPositions Hund Efter Hare:")
for index, (row, col) in enumerate(tile_positions_img2):
    print(f"Tile {index}: Position {row},{col}")

print("\nPositions Gaasetavl")
for index, (row, col) in enumerate(tile_positions_img3):
    print(f"Tile {index}: Position {row},{col}")


# Load image
img = cv2.imread("Gaasetavl/Images/IMG_0845.jpg")

img = cv2.resize(img, (600, 800))

#Grabcut for background removal
# Create an initial mask
mask = np.zeros(img.shape[:2], np.uint8)

# Define a rectangle containing the foreground (board area)
rect = (50, 50, img.shape[1] - 100, img.shape[0] - 100)  # Adjust based on the board size
background = np.zeros((1, 65), np.float64)
foreground = np.zeros((1, 65), np.float64)

# Apply the GrabCut algorithm to segment the foreground (board + pieces)
cv2.grabCut(img, mask, rect, background, foreground, 5, cv2.GC_INIT_WITH_RECT)

# Create a mask where sure background (0) and possible background (2) are set to 0,
# and sure foreground (1) and possible foreground (3) are set to 1
mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')

# Apply mask to extract the foreground (the board and pieces)
img_foreground = img * mask2[:, :, np.newaxis]

# Convert the image to BGRA (to include transparency)
img_transparent = cv2.cvtColor(img_foreground, cv2.COLOR_BGR2BGRA)

# Set alpha (transparency) based on the mask
img_transparent[:, :, 3] = mask2 * 255

#Board and piece detection
img_gray = cv2.cvtColor(img_foreground, cv2.COLOR_BGR2GRAY)

# Gaussian blur
blurred = cv2.GaussianBlur(img_gray, (5, 5), 0)

# Canny edge detection
edges = cv2.Canny(blurred, 50, 150)

# Use morphological operations to clean up the edges and reduce the impact of pieces
kernel = np.ones((5, 5), np.uint8)
edges_cleaned = cv2.morphologyEx(edges, cv2.MORPH_CLOSE, kernel)  # Closing operation to fill gaps

# Find contours from the cleaned edges
contours, _ = cv2.findContours(edges_cleaned, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Check if any contours were found
if len(contours) == 0:
    print("No contours found!")
else:
    # Find the largest contour (this should correspond to the board)
    largest_contour = max(contours, key=cv2.contourArea)
    x_coords = largest_contour.reshape(-1, 2)[:, 0]
    y_coords = largest_contour.reshape(-1, 2)[:, 1]
    # Approximate the contour to simplify the shape
    epsilon = 0.02 * cv2.arcLength(largest_contour, True)
    approx = cv2.approxPolyDP(largest_contour, epsilon, True)
    #print(largest_contour)
    #print(x_coords, y_coords)
    # Calculate the aspect ratio
    x, y, w, h = cv2.boundingRect(largest_contour)
    aspect_ratio = float(w) / h


    # Calculate solidity (ratio of contour area to convex hull area)
    hull = cv2.convexHull(largest_contour)
    hull_area = cv2.contourArea(hull)
    contour_area = cv2.contourArea(largest_contour)
    solidity = contour_area / hull_area

    # Detect a cross-shaped board by checking both aspect ratio and solidity
    if 0.8 < aspect_ratio < 1.2 and 0.5 < solidity < 0.9 and len(approx) > 8:
        board_shape = "Plus Sign Board"
        print("Detected a '+' shaped board.")
    else:
        board_shape = "Unknown Board Shape"
        print("Board not detected as a '+' shaped board.")

    # Now you can proceed to check other properties of the board or contours:
    # 1. Check the area of the board
    board_area = cv2.contourArea(largest_contour)
    print(f"Board area: {board_area}")

    # 2. Check the bounding box dimensions
    x, y, w, h = cv2.boundingRect(largest_contour)
    print(f"Bounding box dimensions: {w}x{h}")
    cv2.drawContours(img, [approx], -1, (0, 255, 0), 3)
    print(x, y, w, h, np.max(x_coords), np.min(x_coords),np.max(y_coords),np.min(y_coords))

    # All points are in format [cols, rows]
    pt_A = [x,y+h]
    pt_B = [x+w, y+h]
    pt_C = [x+w, y]
    pt_D = [x, y]

    # Here, I have used L2 norm. You can use L1 also.
    width_AD = np.sqrt(((pt_A[0] - pt_D[0]) ** 2) + ((pt_A[1] - pt_D[1]) ** 2))
    width_BC = np.sqrt(((pt_B[0] - pt_C[0]) ** 2) + ((pt_B[1] - pt_C[1]) ** 2))
    maxWidth = max(int(width_AD), int(width_BC))

    height_AB = np.sqrt(((pt_A[0] - pt_B[0]) ** 2) + ((pt_A[1] - pt_B[1]) ** 2))
    height_CD = np.sqrt(((pt_C[0] - pt_D[0]) ** 2) + ((pt_C[1] - pt_D[1]) ** 2))
    maxHeight = max(int(height_AB), int(height_CD))

    input_pts = np.float32([pt_A, pt_B, pt_C, pt_D])
    output_pts = np.float32([[0, 0],
                             [0, maxHeight - 1],
                             [maxWidth - 1, maxHeight - 1],
                             [maxWidth - 1, 0]])
    M = cv2.getPerspectiveTransform(input_pts, output_pts)

    out = cv2.warpPerspective(img_gray, M, (maxWidth, maxHeight), flags=cv2.INTER_LINEAR)
    out_col = cv2.warpPerspective(img, M, (maxWidth, maxHeight), flags=cv2.INTER_LINEAR)

    # Hough Circle Transform for piece detection (after detecting the board)
    detected_circles = cv2.HoughCircles(out, cv2.HOUGH_GRADIENT, 0.2, 8, param1=45, param2=21, minRadius=6, maxRadius=15)

    # Dictionary to store the positions of the pieces
    positiondata = {}

    # Variables to count white and black pieces
    white = 0
    black = 0
    circles = 0

    # If circles (pieces) are detected
    if detected_circles is not None:
        detected_circles = np.uint16(np.around(detected_circles))

        for pt in detected_circles[0, :]:
            a, b, r = pt[0], pt[1], pt[2]
            circles += 1

            # Draw the circumference of the circle and the center point
            cv2.circle(out, (a, b), r, (0, 255, 0), 2)
            cv2.circle(out, (a, b), 1, (0, 0, 255), 3)

            # Get the color of the circle (piece)
            mask = np.zeros_like(img_gray)  # Create a mask for the current circle
            cv2.circle(mask, (a, b), r, 255, -1)  # Fill the circle in the mask

            # Extract the colors from the masked area
            masked_img = cv2.bitwise_and(img, img, mask=mask)
            mean_color = cv2.mean(masked_img, mask=mask)[:3]  # Get the mean color

            # Determine if the circle is black or white
            average_color = np.mean(mean_color)  # Calculate average brightness
            if average_color < 122:  # Threshold for black (can adjust)
                black += 1
                positiondata[f"black{black}"] = [int(a), 0, int(b)]
            else:
                white += 1
                positiondata[f"white{white}"] = [int(a), 0, int(b)]

    # Print the results
    print(f"Number of circles detected: {circles}")
    print(f"Position data: {positiondata}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")

    if solidity < 0.9:
        WhatGameIsIt = "Gaasetavl"
    elif aspect_ratio >= 0.95:
        WhatGameIsIt = "Makvaer"
    else:
        WhatGameIsIt = "Hundefterhare"

    print(WhatGameIsIt)

    # Prepare the data to send
    #positions = json.dumps(positiondata)

    # Send the data to the server (assuming the server is expecting this format)
    #sock.sendall(positions.encode("UTF-8"))  # Send position data
    #sock.sendall(board_shape.encode("UTF-8"))  # Send the board shape type
    #receivedData = sock.recv(1024).decode("UTF-8")  # Receiving data from the server
    #print(receivedData)

    print(aspect_ratio)

    # Display the final image with contours and pieces detected
    cv2.imshow("test",out)
    cv2.waitKey(0)

    cv2.destroyAllWindows()