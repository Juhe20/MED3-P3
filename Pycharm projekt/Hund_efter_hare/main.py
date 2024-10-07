import cv2
import numpy as np
import socket
import json

#Host stuff to be able to send data to Unity
host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

#Images
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

#Template matching for the play field
resgray = cv2.matchTemplate(template_gray, img_gray, cv2.TM_CCOEFF_NORMED)
threshold = 0.7  # Adjust threshold as needed
yloc3, xloc3 = np.where(resgray >= threshold)
height3, width3 = template_gray.shape

#Template matches for the white piece
res = cv2.matchTemplate(hvidtemplate_gray, imgplaying_gray, cv2.TM_CCOEFF_NORMED)
threshold = 0.9  # Adjust threshold as needed
yloc1, xloc1 = np.where(res >= threshold)
height1, width1 = hvidtemplate_gray.shape

#Template mathes for the black pieces
res2 = cv2.matchTemplate(sorttemplate_gray, imgplaying_gray, cv2.TM_CCOEFF_NORMED)
threshold2 = 0.95  # Adjust threshold as needed
yloc2, xloc2 = np.where(res2 >= threshold2)
height2, width2 = sorttemplate_gray.shape

for (x, y) in zip(xloc3, yloc3):
    cv2.rectangle(img, (x, y), (x + width3, y + height3), (0, 255, 0), 2)

#Get position of the white piece on the board
hvidposition = []
for (x, y) in zip(xloc1, yloc1):
    cv2.rectangle(imgplaying, (x, y), (x + width1, y + height1), (0, 255, 0), 2)
    positiontuple = [int(x), int(y)]
    hvidposition.append(positiontuple)

# Get position from each black piece on the board
sortposition = []
for index, (x, y) in enumerate(zip(xloc2, yloc2)):
    cv2.rectangle(imgplaying, (x, y), (x + width2, y + height2), (0, 0, 255), 2)
    positiontuple = [int(x), int(y), 0]
    sortposition.append(positiontuple)

#Create empty dictionary to fill in positions
positiondata = {}

# Adding key value pairs for dictionary for each position
for i in range(len(sortposition)):
    positiondata[f"dog{i + 1}"] = (sortposition[i][0], sortposition[i][1])

# Add white position if hvidposition has at least one entry
for i in range(len(hvidposition)):
    positiondata[f"hare{i + 1}"] = (hvidposition[i][0], hvidposition[i][1])

# Convert to JSON
positions = json.dumps(positiondata)

#Send data to Unity
sock.sendall(positions.encode("UTF-8"))  # Converting string to Byte, and sending it to C#
receivedData = sock.recv(1024).decode("UTF-8")  # receiveing data in Byte fron C#, and converting it to String
print(receivedData)





