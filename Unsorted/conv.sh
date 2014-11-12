#!/bin/sh

convert ${1} -transparent '#ff00ff' temp.png
convert temp.png -transparent '#800080' ${2}
rm temp.png
rm ${1}