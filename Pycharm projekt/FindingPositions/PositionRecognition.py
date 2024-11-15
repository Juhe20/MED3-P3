import cv2

def divideStandardImageIntoSections(img, h_size, w_size, ignore_tiles=None): #Method for the standard grid division
    #I have an optional list that includes the tiles that should be ignored. If ignore_tiles wasn't provided then sets it to an empty list
    if ignore_tiles is None:
        ignore_tiles = []

    height, width, _ = img.shape
    #Calculating the height and width of each tile by dividing the image’s dimensions by the specified grid size
    tile_height = height // h_size
    tile_width = width // w_size

    tile_positions = [] #Empty list to store the positions (& if not removed images for each tile)
    for ih in range(h_size): #looping through the grid
        for iw in range(w_size):
            if (ih, iw) in ignore_tiles: #checking if the current position is in the inore tiles list, if yes we skip it
                continue
            # Purely for the display of the tiles alongside their position, remove later
            x = iw * tile_width
            y = ih * tile_height
            tile_img = img[y:y + tile_height, x:x + tile_width]
            tile_positions.append(((ih, iw), tile_img))

            # Display the tile
            cv2.imshow(f"Tile {ih},{iw}", tile_img)
            print(f"Tile at position {ih},{iw}")
            cv2.waitKey(0)
            cv2.destroyAllWindows()
            tile_positions.append((ih, iw))
            # ------------
    return tile_positions #returns the list of tile positions (and for now images)

def divideSpecialImageIntoSections(img, row_lengths): #Method for special division with custom number of columns per row
    height, width, _ = img.shape
    tile_height = height // len(row_lengths) #finding the hight of each row based on the number of rows

    tile_positions = [] #empty list to store the positions
    for ih, row_length in enumerate(row_lengths): #Iterates through row_lengths, where ih is the row index and row_length is the number of tiles in that row
        tile_width = width // row_length
        for iw in range(row_length):
            x = iw * tile_width + 10
            y = ih * tile_height + 10
            # Purely for the display of the tiles alongside their position, remove later
            tile_img = img[y:y + tile_height, x:x + tile_width]
            tile_positions.append(((ih, iw), tile_img))

            # Display the tile
            cv2.imshow(f"Tile {ih},{iw}", tile_img)
            print(f"Tile at position {ih},{iw}")
            cv2.waitKey(0)  # Wait for key press to move to the next tile
            cv2.destroyAllWindows()
            tile_positions.append((ih, iw))
            # -----------------------
    return tile_positions

img1 = cv2.imread("../FindingPositions/Makvaer.png")
img2 = cv2.imread("../FindingPositions/HundEfterHare.png")
img3 = cv2.imread("Gaasetavl.png")

h_size_img1, w_size_img1 = 8, 8
h_size_img2, w_size_img2 = 9, 9
ignore_tiles_HundEfterHare = [
    (0, 0), (0, 1), (0, 2), (0, 3), (0, 5), (0, 6), (0, 7), (0, 8),
    (1, 0), (1, 2), (1, 3), (1, 5), (1, 6), (1, 8),
    (2, 0), (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8),
    (3, 0), (3, 1), (3, 2), (3, 3), (3, 4), (3, 5), (3, 6), (3, 7), (3, 8),
    (4, 2), (4, 3), (4, 5), (4, 6),
    (5, 0), (5, 1), (5, 2), (5, 3), (5, 4), (5, 5), (5, 6), (5, 7), (5, 8),
    (6, 0), (6, 1), (6, 2), (6, 3), (6, 4), (6, 5), (6, 6), (6, 7), (6, 8),
    (7, 0), (7, 2), (7, 3), (7, 5), (7, 6), (7, 8),
    (8, 0), (8, 1), (8, 2), (8, 3), (8, 5), (8, 6), (8, 7), (8, 8)
]

#tile_positions_img1 = divideStandardImageIntoSections(img1, h_size_img1, w_size_img1)
#tile_positions_img2 = divideStandardImageIntoSections(img2, h_size_img2, w_size_img2, ignore_tiles_HundEfterHare)

row_lengths_img3 = [7, 6, 7, 6, 7, 6, 7, 6]  #Defining the number of columns for each row
tile_positions_img3 = divideSpecialImageIntoSections(img3, row_lengths_img3)

#Printing positions for each image
#print("Positions Makvaer:")
#for index, (row, col) in enumerate(tile_positions_img1):
#      print(f"Tile {index}: Position {row},{col}")

#print("\nPositions Hund Efter Hare:")
#for index, (row, col) in enumerate(tile_positions_img2):
#    print(f"Tile {index}: Position {row},{col}")

print("\nPositions Gaasetavl")
for index, (row, col) in enumerate(tile_positions_img3):
    print(f"Tile {index}: Position {row},{col}")
