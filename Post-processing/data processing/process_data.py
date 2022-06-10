from genericpath import exists
from os import walk
from pathlib import Path
import os
import cv2
import re
import sys
import pandas as pd  
import numpy as np


# Constants
PATH = '.\\'
COLUMNS = ["File", "Subject", "Scenario", "Frame", "Label"]
OUTPUT_PATH = '.\data'
LABELS = ["blue", "green", "pink", "purple", "red", "yellow"]

file_data = dict([(key, []) for key in LABELS])

if not VERBOSE: sys.stdout = open('.\logs.txt', 'w')


for (path, directories, files) in walk(PATH):
     
    for file in files:
        
        if not file.lower().endswith(('.jpg')): continue
        
        file_info = Path(file).stem.split("_")
        
        FILE = os.path.abspath(os.path.join(path, file))
        SUBJECT = file_info[0].replace("subject", "") 
        SCENARIO = file_info[1].replace("scenario", "") 
        FRAME = file_info[2].replace("frame", "") 
        LABEL = file_info[3]
        
        file_data[LABEL].append([FILE, SUBJECT, SCENARIO, FRAME, LABEL])
                     
for key in file_data.keys():
    
    df = pd.DataFrame(file_data[key], columns=COLUMNS)
    
    df.to_csv(os.path.join(OUTPUT_PATH + "/" + key + '.csv'), index=False)      
    
    print(df)
    print("\n\n")
    
if not VERBOSE:  sys.stdout.close()
  