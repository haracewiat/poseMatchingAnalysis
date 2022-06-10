from os import walk
import os
import json
import sys
from pathlib import Path
import time
import pandas as pd  
import numpy as np

PATH = '.\\csv\\'

for (path, directories, files) in walk(PATH):
          
    for file in files:
        
        if not file.lower().endswith(('.csv')): continue
        
        
        df = pd.read_csv(os.path.join(path, file))
        new_df = pd.DataFrame(columns=df.columns)
        
        print(file)        
        
        for index, row in df.iterrows():
            
            file_name = row["File"]            
            
            file_info = file_name.split("_")
        
            subject = file_info[0]
            scenario = file_info[1]
            frame = file_info[2]            
            frame_number = int(frame.replace("frame", ""))
        
            new_name = subject + "_" + scenario + "_frame" + str(frame_number - 1)
            row["File"] = new_name
            
            new_df = new_df.append(row)
        
        new_df.to_csv(os.path.join(path, file), index=False)
        
        # file_info = file.split("_")
        
        # subject = file_info[0]
        # scenario = file_info[1]
        # frame = file_info[2].replace(".jpg", "")
        # extension = ".jpg"
        
        # frame_number = int(frame.replace("frame", ""))
        
        
        # os.rename(os.path.join(path, file), os.path.join(".\\renamed\\", subject + "_" + scenario + "_frame" + str(frame_number - 1) + extension))
        # print(subject + "_" + scenario + "_frame" + str(frame_number - 1) + extension)
        