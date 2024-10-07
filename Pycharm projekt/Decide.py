import cv2
import numpy as np
import socket
import json

host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

#img = cv2.imread("Makvaer/Makvaer.png")
img = cv2.imread("Hund_efter_hare/boardplaying.png")
#img = cv2.imread("Gaasetavl/Images/Gaasetavl.png")

img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

gray_blurred = cv2.blur(img_gray, (3, 3))

# Apply Hough transform on the blurred image.
detected_circles = cv2.HoughCircles(gray_blurred,
                                    cv2.HOUGH_GRADIENT, 0.2, 20, param1=55,
                                    param2=30, minRadius=1, maxRadius=30)

positiondata = {}
# Draw circles that are detected.
if detected_circles is not None:

    # Convert the circle parameters a, b and r to integers.
    detected_circles = np.uint16(np.around(detected_circles))
    white  = 0
    black = 0
    circles = 0
    colors = []
    for pt in detected_circles[0, :]:
        a, b, r = pt[0], pt[1], pt[2]
        circles = circles+1

        # Draw the circumference of the circle.
        #cv2.circle(img, (a, b), r, (0, 255, 0), 2)


        # Draw a small circle (of radius 1) to show the center.
        cv2.circle(img, (a, b), 1, (0, 0, 255), 3)



#Lavet selv og med https://www.geeksforgeeks.org/circle-detection-using-opencv-python/
        # Get the color of the circle
        mask = np.zeros_like(img_gray)  # Create a mask for the current circle
        cv2.circle(mask, (a, b), r, 255, -1)  # Fill the circle in the mask

        # Extract the colors from the masked area
        masked_img = cv2.bitwise_and(img, img, mask=mask)
        mean_color = cv2.mean(masked_img, mask=mask)[:3]  # Get the mean color
        colors.append(mean_color)  # Store the mean color

        # Determine if the circle is black or white
        average_color = np.mean(mean_color)  # Calculate average brightness
        if average_color < 128:  # Threshold for black (can adjust)
            black += 1
            positiondata[f"black{black}"] = [int(a), 0, int(b)]
        else:
            white += 1
            positiondata[f"white{white}"] = [int(a), 0, int(b)]
#Lavet af Chat


    print(f"Number of circles: {circles}")
    print(f"Positiondata: {positiondata}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")

if white == 1 and black == 3:
    WhatGameIsIt = "Hundefterhare"
elif white == 2:
    WhatGameIsIt = "Gaasetavl"
else:
    WhatGameIsIt = "Makvaer"

positions = json.dumps(positiondata)

sock.sendall(positions.encode("UTF-8"))  # Converting string to Byte, and sending it to C#
sock.sendall(WhatGameIsIt.encode("UTF-8"))
receivedData = sock.recv(1024).decode("UTF-8")  # receiving data in Byte from C#, and converting it to String
print(receivedData)

print(WhatGameIsIt)