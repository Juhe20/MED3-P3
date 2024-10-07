from asyncio import to_thread

import cv2
import numpy as np
import json


def detectAndCountCircles(path):
    inp_img = cv2.imread(path)

    gray = cv2.cvtColor(inp_img, cv2.COLOR_BGR2GRAY)

    gray_blurred = cv2.blur(gray, (3, 3))

    detected_circles = cv2.HoughCircles(gray_blurred,
                                        cv2.HOUGH_GRADIENT,
                                        1,
                                        20,
                                        param1=50,
                                        param2=30,
                                        minRadius=10,
                                        maxRadius=30)

    if detected_circles is not None:
        detected_circles = np.uint16(np.around(detected_circles))
        total_circles = 0
        foxes = 0
        geese = 0
        threshold = 127
        pawn_list = {}
        pawn_type = ""
        pawn_id = 0

        for pt in detected_circles[0, :]:
            total_circles += 1
            x, y, r = pt[0], pt[1], pt[2]
            dot_color = (0, 0, 255)
            circle_color = (255, 0, 0)

            # checking pawn color
            mask = np.zeros(inp_img.shape[:2], dtype="uint8")
            cv2.circle(mask, (x, y), r, 255, -1)

            masked_img = cv2.bitwise_and(inp_img, inp_img, mask=mask)
            mean_color = cv2.mean(inp_img, mask=mask)
            B, G, R = mean_color[:3]
            mean_intensity = (R + G + B) / 3

            if mean_intensity > threshold:
                foxes += 1
                pawn_type = "white"
            elif mean_intensity < threshold:
                geese += 1
                pawn_type = "black"

            pawn_list[f'({pawn_type},{pawn_id})'] = (int(x),int(y))
            pawn_id += 1
        return pawn_list
    else:
        print("Error: detected_circles = None")
        return None

def dictToJson(dict):
    json_string = json.dumps(dict, indent=4)
    return json_string

print(dictToJson(detectAndCountCircles("../Gaasetavl/Images/gaasetavl 1.png")))