import cv2
import numpy as np

img = cv2.imread("Makvaer1.png", cv2.IMREAD_COLOR)
# Convert to grayscale
gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

# Blur using 3 * 3 kernel.
gray_blurred = cv2.blur(gray, (3, 3))

# Apply Hough transform on the blurred image.
detected_circles = cv2.HoughCircles(gray_blurred,
                                    cv2.HOUGH_GRADIENT, 1, 20, param1=50,
                                    param2=30, minRadius=1, maxRadius=40)

# Draw circles that are detected.
if detected_circles is not None:

    # Convert the circle parameters a, b and r to integers.
    detected_circles = np.uint16(np.around(detected_circles))
    white  = 0
    black = 0
    circles = 0
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
        mask = np.zeros_like(gray)  # Create a mask for the current circle
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


    #Find the board
    edges = cv2.Canny(gray, 50, 150, apertureSize=3)
    Lines = []
    y_line = []
    x_line = []

    # This returns an array of r and theta values
    lines = cv2.HoughLines(edges, 1, np.pi / 180, 210)
    # The below for loop runs till r and theta values
    # are in the range of the 2d array
    for r_theta in lines:
        arr = np.array(r_theta[0], dtype=np.float64)
        r, theta = arr
        # Stores the value of cos(theta) in a
        a = np.cos(theta)

        # Stores the value of sin(theta) in b
        b = np.sin(theta)

        # x0 stores the value rcos(theta)
        x0 = a * r

        # y0 stores the value rsin(theta)
        y0 = b * r

        # x1 stores the rounded off value of (rcos(theta)-1000sin(theta))
        x1 = int(x0 + 1000 * (-b))

        # y1 stores the rounded off value of (rsin(theta)+1000cos(theta))
        y1 = int(y0 + 1000 * (a))

        # x2 stores the rounded off value of (rcos(theta)+1000sin(theta))
        x2 = int(x0 - 1000 * (-b))

        # y2 stores the rounded off value of (rsin(theta)-1000cos(theta))
        y2 = int(y0 - 1000 * (a))
        #print(x1, y1, x2, y2)

        y_line.append(y1)
        y_line.append(y2)
        x_line.append(x1)
        x_line.append(x2)

        Lines.append([y_line, x_line])

        # cv2.line draws a line in img from the point(x1,y1) to (x2,y2).
        # (0,0,255) denotes the colour of the line to be
        # drawn. In this case, it is red.
        cv2.line(img, (x1, y1), (x2, y2), (0, 0, 255), 2)


    #Fundet på https://www.geeksforgeeks.org/line-detection-python-opencv-houghline-method/


    print(f"Number of circles: {circles}")
    print(f"Each circles position: {pos}")
    print(f"Number of white circles: {white}")
    print(f"Number of black circles: {black}")
    print(f"Number of lines: {Lines}")
cv2.imshow("Detected Circle", img)
cv2.imshow("Edges", edges)
cv2.waitKey(0)