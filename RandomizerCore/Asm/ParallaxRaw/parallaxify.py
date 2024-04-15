#!/usr/bin/env python3
#
# Bitmap to multi-console CHR converter using Pillow
# (with PB8 instead of PackBits)
#
# Copyright 2014-2015 Damian Yerrick
# Copying and distribution of this file, with or without
# modification, are permitted in any medium without royalty
# provided the copyright notice and this notice are preserved.
# This file is offered as-is, without any warranty.

from PIL import Image
from time import sleep
import array

# python 2/3 cross compatibility fixes
try:
    xrange
except NameError:
    xrange = range
try:
    raw_input
except NameError:
    raw_input = input
try:
    next
except NameError:
    next = lambda x: x.next()

def formatTilePlanar(tile, planemap, hflip=False, little=False):
    """Turn a tile into bitplanes.

Planemap opcodes:
10 -- bit 1 then bit 0 of each tile
0,1 -- planar interleaved by rows
0;1 -- planar interlaved by planes
0,1;2,3 -- SNES/PCE format

"""
    hflip = 7 if hflip else 0
    if (tile.size != (8, 8)):
        return None
    pixels = list(tile.getdata())
    pixelrows = [pixels[i:i + 8] for i in xrange(0, 64, 8)]
    if hflip:
        for row in pixelrows:
            row.reverse()
    out = bytearray()

    planemap = [[[int(c) for c in row]
                 for row in plane.split(',')]
                for plane in planemap.split(';')]
    # format: [tile-plane number][plane-within-row number][bit number]

    # we have five (!) nested loops
    # outermost: separate planes
    # within separate planes: pixel rows
    # within pixel rows: row planes
    # within row planes: pixels
    # within pixels: bits
    for plane in planemap:
        for pxrow in pixelrows:
            for rowplane in plane:
                rowbits = 1
                thisrow = bytearray()
                for px in pxrow:
                    for bitnum in rowplane:
                        rowbits = (rowbits << 1) | ((px >> bitnum) & 1)
                        if rowbits >= 0x100:
                            thisrow.append(rowbits & 0xFF)
                            rowbits = 1
                if little: thisrow.reverse()
                out.extend(thisrow)
    return out

def pilbmp2chr(im, tileWidth=8, tileHeight=8,
               formatTile=lambda im: formatTilePlanar(im, "0;1")):
    """Convert a bitmap image into a list of byte strings representing tiles."""
    im.load()
    (w, h) = im.size
    outdata = []
    for mt_y in range(0, h, tileHeight):
        for mt_x in range(0, w, tileWidth):
            metatile = im.crop((mt_x, mt_y,
                                mt_x + tileWidth, mt_y + tileHeight))
            for tile_y in range(0, tileHeight, 8):
                for tile_x in range(0, tileWidth, 8):
                    tile = metatile.crop((tile_x, tile_y,
                                          tile_x + 8, tile_y + 8))
                    data = formatTile(tile)
                    outdata.append(data)
    return outdata

def parallaxify():
    # due to absolute laziness, this loads parallax.bmp in the same folder
    # and generates 32 horizontally scrolled versions of the background
    import os
    curpath = os.path.abspath(os.path.dirname(__file__))
    orig_img = Image.open(f"{curpath}/parallax_in.bmp")

    im = orig_img.copy()
    im = im.resize((128, 32))
    
    outdata = b''
    
    width = 16
    for i in range(0, width):
        # shift the main image over by i pixels
        tiles = 10
        shifted = orig_img.crop((width - i, 0, (width * 2) - i, width * tiles))
        # clear the image
        im.paste(0, (0,0,128,32))
        for j in range(0, tiles):
            # print( [16*x for x in (0, 0 + j, 1, 1 + j)] )
            # region = shifted.crop([16*x for x in (0, 0 + j, 1, 1 + j)])
            print( [8*x for x in (0 + (j % 8), 0 + (j // 8), 1 + (j % 8), 1 + (j // 8))] )
            # arrange the tiles so that they look good in a 8x16 view
            # top left
            top_left_region = shifted.crop([8*x for x in (0, 0 + j*2, 1, 1 + j*2)])
            im.paste(top_left_region, [8*x for x in (0 + 4 * (j % 4), 0 + (j // 4), 1 + 4 * (j % 4), 1 + (j // 4))])
            # bot left
            bot_left_region = shifted.crop([8*x for x in (0, 1 + j*2, 1, 2 + j*2)])
            im.paste(bot_left_region, [8*x for x in (1 + 4 * (j % 4), 0 + (j // 4), 2 + 4 * (j % 4), 1 + (j // 4))])
            # top right
            top_right_region = shifted.crop([8*x for x in (1, 0 + j*2, 2, 1 + j*2)])
            im.paste(top_right_region, [8*x for x in (2 + 4 * (j % 4), 0 + (j // 4), 3 + 4 * (j % 4), 1 + (j // 4))])
            # bot right
            bot_right_region = shifted.crop([8*x for x in (1, 1 + j*2, 2, 2 + j*2)])
            im.paste(bot_right_region, [8*x for x in (3 + 4 * (j % 4), 0 + (j // 4), 4 + 4 * (j % 4), 1 + (j // 4))])
        out = b''.join(pilbmp2chr(im))
        out += bytes(1024 - len(out))
        outdata += out
    with open(f"{curpath}/../parallax.chr", "wb") as fout:
        fout.write(outdata)


if __name__ == "__main__":
    parallaxify()
