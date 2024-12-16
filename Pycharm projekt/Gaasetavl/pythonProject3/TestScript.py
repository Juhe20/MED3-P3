import cv2 as cv
import numpy as np

# Read the original image
im = cv.imread("../Gaasetavl/Images/Gaasetavl.png")

# Convert to grayscale
gray = cv.cvtColor(im, cv.COLOR_BGR2GRAY)

# Perform edge detection using Canny
edges = cv.Canny(gray, 20, 200, apertureSize=5)  # Using an edge detection algorithm

# Create a copy of the original image to draw lines on
im_with_lines = im.copy()

# Apply Hough Line Transform to detect lines
lines = cv.HoughLines(edges, 1, np.pi / 180, 100)

if lines is not None:
    for line in lines:
        rho, theta = line[0]
        a = np.cos(theta)
        b = np.sin(theta)
        x0 = a * rho
        y0 = b * rho
        x1 = int(x0 + 1000 * (-b))
        y1 = int(y0 + 1000 * (a))
        x2 = int(x0 - 1000 * (-b))
        y2 = int(y0 - 1000 * (a))

        # Draw the detected lines on the copy of the image
        cv.line(im_with_lines, (x1, y1), (x2, y2), (0, 0, 255), 2)

# Save the modified copy with lines drawn
cv.imwrite('detected_lines.png', im_with_lines)

# Display the modified copy with detected lines
cv.imshow("Detected Lines", im_with_lines)
cv.waitKey(0)
cv.destroyAllWindows()