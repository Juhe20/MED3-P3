import cv2
import numpy as np
from cv2 import waitKey
import socket
import time
import json

host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

img = cv2.imread("board.png")
img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
template = cv2.imread("template.png")
template_gray = cv2.cvtColor(template, cv2.COLOR_BGR2GRAY)
imgplaying = cv2.imread("boardplaying.png ")
imgplaying_gray = cv2.cvtColor(imgplaying, cv2.COLOR_BGR2GRAY)
hvidtemplate = cv2.imread("hvid.png")
hvidtemplate_gray = cv2.cvtColor(hvidtemplate, cv2.COLOR_BGR2GRAY)
sorttemplate = cv2.imread("sort.png")
sorttemplate_gray = cv2.cvtColor(sorttemplate, cv2.COLOR_BGR2GRAY)

#Match play area
resgray = cv2.matchTemplate(template_gray, img_gray, cv2.TM_CCOEFF_NORMED)
threshold = 0.7  # Adjust threshold as needed
yloc3, xloc3 = np.where(resgray >= threshold)
height3, width3 = template_gray.shape

#Match hvid
res = cv2.matchTemplate(hvidtemplate_gray, imgplaying_gray, cv2.TM_CCOEFF_NORMED)
threshold = 0.9  # Adjust threshold as needed
yloc1, xloc1 = np.where(res >= threshold)
height1, width1 = hvidtemplate_gray.shape

#Match sort
res2 = cv2.matchTemplate(sorttemplate_gray, imgplaying_gray, cv2.TM_CCOEFF_NORMED)
threshold2 = 0.95  # Adjust threshold as needed
yloc2, xloc2 = np.where(res2 >= threshold2)
height2, width2 = sorttemplate_gray.shape

for (x, y) in zip(xloc3, yloc3):
    cv2.rectangle(img, (x, y), (x + width3, y + height3), (0, 255, 0), 2)

hvidposition = []
for (x, y) in zip(xloc1, yloc1):
    cv2.rectangle(imgplaying, (x, y), (x + width1, y + height1), (0, 255, 0), 2)
    positiontuple = [x,y]
    hvidposition.append(positiontuple)

sortposition = []
for (x, y) in zip(xloc2, yloc2):
    cv2.rectangle(imgplaying, (x, y), (x + width2, y + height2), (0, 0, 255), 2)
    positiontuple = [int(x),int(y),0]
    sortposition.append(positiontuple)

positiondata = {
    "blackposition1": (int(sortposition[0][0]), int(sortposition[0][1])),
    "blackposition2": (int(sortposition[1][0]), int(sortposition[1][1])),
    "blackposition3": (int(sortposition[2][0]), int(sortposition[2][1])),
    "whiteposition1": (int(hvidposition[0][0]), int(hvidposition[0][1])),
}
positions = json.dumps(positiondata)


#sortposition[0][0] += 1  # Increment x
#sortposition[0][1] += 1  # Increment y
#posString = ','.join(map(lambda v: f"{','.join(map(str, v))}", sortposition))
#print(posString)

sock.sendall(positions.encode("UTF-8"))  # Converting string to Byte, and sending it to C#
receivedData = sock.recv(1024).decode("UTF-8")  # receiveing data in Byte fron C#, and converting it to String
print(receivedData)





