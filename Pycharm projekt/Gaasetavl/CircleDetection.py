from asyncio import to_thread

import cv2
import numpy as np

inp_img = cv2.imread("../Gaasetavl/Images/Gaasetavl 6.png")

gray = cv2.cvtColor(inp_img, cv2.COLOR_BGR2GRAY)

gray_blurred = cv2.blur(gray,(3,3))

detected_circles = cv2.HoughCircles(gray_blurred,
                                   cv2.HOUGH_GRADIENT,
                                   1,
                                   20,
                                   param1 = 50,
                                   param2 = 30,
                                   minRadius=10,
                                   maxRadius=20)

if detected_circles is not None:
    detected_circles = np.uint16(np.around(detected_circles))
    total_circles = 0
    foxs = 0
    geese = 0
    threshold1 = -1
    threshold2 = -1

    for pt in detected_circles[0,:]:
        total_circles += 1
        a,b,r = pt[0], pt[1], pt[2]
        dot_color = (0,0,255)
        circle_color = (255,0,0)

        #checking pawn color
        mask = np.zeros(inp_img.shape[:2], dtype="uint8")
        cv2.circle(mask, (a, b), r, 255, -1)

        masked_img = cv2.bitwise_and(inp_img, inp_img, mask=mask)
        mean_color = cv2.mean(inp_img, mask=mask)
        B, G, R = mean_color[:3]
        mean_intensity = (R+G+B) / 3

        tolerance = 15 #a tolerance for if the first circle detected isn't the highest/lowest intensity

        #if both thresholds have been set it will check the intensity and add geese/foxs accordingly
        if threshold1 != -1 and threshold2 != -1:
            if mean_intensity - tolerance < threshold1:
                geese += 1
            elif mean_intensity + tolerance > threshold2:
                foxs += 1

        #if the thresholds haven't been set
        #starts with checking if threshold1 is set, if not we set it to mean_intensity, assumes it is black and add 1 to geese
        if threshold1 == -1:
            threshold1 = mean_intensity
            geese += 1
        #then on the next iteration we first check if the mean_intensity + our tolerence is smaller than our threshold1
        elif threshold2 == -1 and mean_intensity + tolerance < threshold1:
            # if it is we set our threshold2 to threshold1 and threshold1 to the current mean_intensity because we want threshold1 to be the lowest value
            threshold2 = threshold1 - tolerance
            threshold1 = mean_intensity + tolerance
            #if the first 1 or first two was white, and we then find a black, we can then set the foxs to geese
            #and subtract the foxs-1 from the geese, because the new we found was black so if we just subtracted white we would be 1 black short
            foxs = geese
            geese -= foxs - 1
        #if the threshold is greater than it means the first was black and we have now found a white, so we set threshold2 to the mean_intensity
        elif threshold2 == -1 and mean_intensity - tolerance > threshold1:
            threshold2 = mean_intensity - tolerance
            foxs = 1
        #if none of the above is true, it means we are still looking at a black and we can add one to geese
        elif threshold2 == -1:
            geese += 1

        #drawing the circle around
        cv2.circle(inp_img, (a, b), r, circle_color, 2)

        #drawing the circle in the center
        cv2.circle(inp_img,(a,b),1,dot_color,3)

    cv2.imshow("Detected Circle", inp_img)
    print(f'Total: {total_circles}\nfoxs: {foxs}\ngeese: {geese}\nt1: {threshold1}\t t2: {threshold2}')
    cv2.waitKey(0)