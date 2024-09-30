import cv2
import numpy as np

#img = cv2.imread("Makvaer/Makvaer.png")
img = cv2.imread("Hund_efter_hare/boardplaying.png")
img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

gray_blurred = cv2.blur(img_gray, (3, 3))

# Apply Hough transform on the blurred image.
detected_circles = cv2.HoughCircles(gray_blurred,
                                    cv2.HOUGH_GRADIENT, 0.2, 20, param1=55,
                                    param2=30, minRadius=1, maxRadius=30)

# Draw circles that are detected.
if detected_circles is not None:

    # Convert the circle parameters a, b and r to integers.
    detected_circles = np.uint16(np.around(detected_circles))
    white  = 0
    black = 0
    circles = 0
    Numlines = 0
    pos = []
    x = []
    y = []
    colors = []

    for pt in detected_circles[0, :]:
        a, b, r = pt[0], pt[1], pt[2]
        circles = circles+1

        # Draw the circumference of the circle.
        #cv2.circle(img, (a, b), r, (0, 255, 0), 2)


        # Draw a small circle (of radius 1) to show the center.
        cv2.circle(img, (a, b), 1, (0, 0, 255), 3)
        x.append(a)
        y.append(b)
        pos = [x,y]
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
        else:
            white += 1
#Lavet af Chat

    print(f"Number of circles: {circles}")
    print(f"Each circles position: {pos}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")

if(white == 2):
    WhatGameIsIt = "Gaasetavl"
if(white == 1 and black == 3):
    WhatGameIsIt = "HundEfterHare"
else:
    WhatGameIsIt = "Makvaer"

print(WhatGameIsIt)
cv2.imshow("Detected Circle", img)
cv2.waitKey(0)
