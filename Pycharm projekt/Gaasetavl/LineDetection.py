import cv2 as cv
import numpy as np
import math

# Read the original image in color
im = cv.imread("../Gaasetavl/Images/Gaasetavl.png")

# Convert to grayscale and threshold
gray = cv.cvtColor(im, cv.COLOR_BGR2GRAY)
ret, bw_img = cv.threshold(gray, 195, 255, cv.THRESH_BINARY)

# Perform edge detection using Canny
edges = cv.Canny(bw_img, 100, 200, apertureSize=5)

# Create a copy of the original image to draw lines on
im_with_lines = im.copy()  # Use the color image here

# Apply Probabilistic Hough Line Transform
min_line_length = 10
max_line_gap = 70     # Adjust the maximum gap allowed between line segments to be considered a single line

linesP = cv.HoughLinesP(edges, 1, np.pi / 180, threshold=70, minLineLength=min_line_length, maxLineGap=max_line_gap)

if linesP is not None:
    prev_x1 = 0
    prev_y1 = 0
    for line in linesP:
        x1, y1, x2, y2 = line[0]
        # Draw the detected line segments on the copy of the color image
        cv.line(im_with_lines, (x1, y1), (x2, y2), (0, 0, 255), 5)  # Red color for lines

# Save the modified copy with lines drawn
cv.imwrite('detected_lines.png', im_with_lines)

# Display the modified copy with detected lines
cv.imshow("Detected Lines", im_with_lines)
print(linesP)
cv.waitKey(0)
cv.destroyAllWindows()
