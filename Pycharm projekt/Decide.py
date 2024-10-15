import cv2
import numpy as np
import socket
import json

# Connect to the server
host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

# Load the image (you can uncomment whichever board you want to test)
#img = cv2.imread("Hund_efter_hare/boardplaying.png")
#img = cv2.imread("Makvaer/Capture.PNG")
img = cv2.imread("Gaasetavl/Images/Gaasetavl 5.png")

# Convert the image to grayscale
img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

# Apply Gaussian blur to reduce noise
blurred = cv2.GaussianBlur(img_gray, (5, 5), 0)

# Apply Canny edge detection to the blurred image
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

    # Approximate the contour to reduce the number of points and simplify the shape
    # Approximate the contour to simplify the shape
    epsilon = 0.02 * cv2.arcLength(largest_contour, True)
    approx = cv2.approxPolyDP(largest_contour, epsilon, True)

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

    # Optionally, you can now proceed with further processing, e.g., detecting the pieces
    # Hough Circle Transform for piece detection (after detecting the board)
    detected_circles = cv2.HoughCircles(blurred, cv2.HOUGH_GRADIENT, 0.2, 20, param1=55, param2=30, minRadius=1, maxRadius=30)

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
            cv2.circle(img, (a, b), r, (0, 255, 0), 2)
            cv2.circle(img, (a, b), 1, (0, 0, 255), 3)

            # Get the color of the circle (piece)
            mask = np.zeros_like(img_gray)  # Create a mask for the current circle
            cv2.circle(mask, (a, b), r, 255, -1)  # Fill the circle in the mask

            # Extract the colors from the masked area
            masked_img = cv2.bitwise_and(img, img, mask=mask)
            mean_color = cv2.mean(masked_img, mask=mask)[:3]  # Get the mean color

            # Determine if the circle is black or white
            average_color = np.mean(mean_color)  # Calculate average brightness
            if average_color < 128:  # Threshold for black (can adjust)
                black += 1
                positiondata[f"black{black}"] = [int(a), 0, int(b)]
            else:
                white += 1
                positiondata[f"white{white}"] = [int(a), 0, int(b)]

    cv2.drawContours(img, [approx], -1, (0, 255, 0), 3)

    # Print the results
    print(f"Number of circles detected: {circles}")
    print(f"Position data: {positiondata}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")

    if solidity<0.9:
        WhatGameIsIt = "Gaasetavl"
    elif aspect_ratio == 1:
        WhatGameIsIt = "Makvaer"
    else:
        WhatGameIsIt = "Hundefterhare"

    print(WhatGameIsIt)

    # Prepare the data to send
    positions = json.dumps(positiondata)

    # Send the data to the server (assuming the server is expecting this format)
    sock.sendall(positions.encode("UTF-8"))  # Send position data
    sock.sendall(board_shape.encode("UTF-8"))  # Send the board shape type
    receivedData = sock.recv(1024).decode("UTF-8")  # Receiving data from the server
    print(receivedData)


    # Show the final image with detected pieces and board
    cv2.imshow("Detected Board Contour", img)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
