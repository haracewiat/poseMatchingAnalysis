from genericpath import exists
from os import walk
from pathlib import Path
import os
import cv2
import re
import sys

sys.stdout = open('logs.txt', 'w')

# Constants
PATH = '.\\ProcessedData'

for (path, directories, files) in walk(PATH):
     
    for file in files:
        
        LABEL = os.path.basename(path)
                
        if file.count('_') == 3 and Path(file).stem.split("_")[-1] == LABEL:
            print ("[" + os.path.join(path, file) + "] " + file + " already contains the \'" + LABEL + "\' label.")
            
        elif  file.count('_') == 3 and not re.match('[^_]+label', Path(file).stem):
            print ("[" + os.path.join(path, file) + "] " + file + " already contains a different label.")
            
        else:
            print("[" + os.path.join(path, file) + "] " + "Renaming file " + file + " to " + Path(file).stem + "_" + LABEL + Path(file).suffix)
            os.rename(os.path.join(path, file), os.path.join(path, Path(file).stem + "_" + LABEL + Path(file).suffix))
            pass
    
        
sys.stdout.close()
  