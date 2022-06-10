from os import walk
import os
import json
import sys
from pathlib import Path
import time
import pandas as pd

VERBOSE = False

if not VERBOSE: sys.stdout = open('.\logs.txt', 'w')

# Step 1: Load all the old csv files
OLD_CSV_PATH = '.\\csv_old\\'
old_csv_files = []

for (path, directories, files) in walk(OLD_CSV_PATH):          
    for file in files:
        if not file.lower().endswith(('.csv')): continue
        
        df = pd.read_csv(os.path.join(path, file))        
        old_csv_files.append(df)

print(len(old_csv_files))


# Step 2: Load all the new csv files
NEW_CSV_PATH = '.\\csv_new\\'
new_csv_files = []

for (path, directories, files) in walk(NEW_CSV_PATH):          
    for file in files:
        if not file.lower().endswith(('.csv')): continue

        df = pd.read_csv(os.path.join(path, file))
        new_csv_files.append(df)

print(len(new_csv_files))


# Step 3: For each file in the directory, find an entry in the dataframe and relabel accordingly
IMAGES_PATH = '.\\images\\'
OUTPUT_PATH = '.\\output\\'
COLUMNS = ["File", "Subject", "Scenario", "Frame", "Label", "Detection Score", "Landmarks"]

LABELS = ["blue", "green", "pink", "purple", "red", "yellow"]
new_entries = dict([(key, []) for key in LABELS])

for (path, directories, files) in walk(IMAGES_PATH):
    for file in files:        
        if not file.lower().endswith(('.jpg')): continue
          
          
        keyword = Path(file).stem
        
        old_row = []
        new_row = [] 
        
        print(keyword)
        
        # Get old row
        for df in old_csv_files:       
             
            entries = df.loc[df["File"].str.contains(keyword + "_")]    
            
            if len(entries) != 0: 
                old_row = entries          
                break   
        
        # Get new row
        for df in new_csv_files:       
             
            entries = df.loc[df["File"] == keyword]    
            
            if len(entries) != 0:
                new_row = entries
                break
            
        # If the image doesn't exist in the old dataframe, delete it (it had no label)
        if len(old_row) == 0 or len(new_row) == 0 :
            print("Removing...\n\n")
            os.rename(os.path.join(path, file), os.path.join(OUTPUT_PATH + "images\\delete\\", file))
           
        else:  
            # Else, find the label for the file 
            file_name = old_row.values[0][0]
            file_info = file_name.split("_")
            label = file_info[-1].replace(".jpg", "")
            
            print("Asigning label " + label + ".\n\n")
            
            # Move the file to respective folder
            os.rename(os.path.join(path, file), os.path.join(OUTPUT_PATH + "images\\" + label + "\\", keyword + "_" + label + ".jpg"))

            
            file_info = keyword.split("_")
            subject = file_info[0]
            scenario = file_info[1]
            frame = file_info[2].replace(".jpg", "")
            frame_number = int(frame.replace("frame", ""))
            
            
            new_entries[label].append([keyword + "_" + label  + ".jpg", subject, scenario, frame_number, label, new_row.values[0][1], new_row.values[0][2]])
            
        print("\n") 
            
# Step 4: Create new dataframes
for key in new_entries.keys():
    
    df = pd.DataFrame(new_entries[key], columns=COLUMNS)
        
    df.to_csv(os.path.join(OUTPUT_PATH + "csv\\" + key + '.csv'), index=False)      
    
    print(df)
    print("\n\n")  
    
    
if not VERBOSE:  sys.stdout.close()