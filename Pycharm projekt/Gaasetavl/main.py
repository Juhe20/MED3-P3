import cv2


im = cv2.imread("Images/Gaasetavl - blue dots.png",0) #Loading the image, converting it to grayscale
th, res = cv2.threshold(im, 113, 255, cv2.THRESH_BINARY) #taking the input image, applying the binary thresholding to it; setting a threshold so that all values bellow 113 becomes 0(black), pixels above the threshold are set to 255(whitew)
#return values:
#th = the threshold (here,113)
#res = the output, binary image

# Load image
im = cv2.imread("Images/Gaasetavl - blue dots.png", 0)

# Set the lower and upper thresholds
lower_threshold = 249
upper_threshold = 250

# Create a binary mask using the lower and upper thresholds
_, res_lower = cv2.threshold(im, lower_threshold, 255, cv2.THRESH_BINARY)
_, res_upper = cv2.threshold(im, upper_threshold, 255, cv2.THRESH_BINARY_INV)

# Combine the two masks
res2 = cv2.bitwise_and(res_lower, res_upper)

# Apply a median filter to reduce the salt-and-pepper noice
median = cv2.medianBlur(res2,5)

# Show the images
cv2.imshow("Original Image", im)
cv2.imshow("Binary image, player 2", res)
cv2.imshow("Median Filtered Image, player 1", median)
cv2.waitKey(0)