from os import walk
import os
import json
import sys
from pathlib import Path
import time
import pandas as pd
import numpy as np
from sklearn.metrics import accuracy_score
from sklearn.metrics import confusion_matrix
import seaborn as sns
import matplotlib.pyplot as plt
from collections import Counter


for (path, directories, files) in walk(".\\csv\\"):          
    for file in files:
        if not file.lower().endswith(('.csv')): continue
        
        label = Path(file).stem
        
        df = pd.read_csv(os.path.join(path, file))
        
        print(label)
        files_to_delete = []
        
        for (path2, directories2, files2) in walk(".\\" + label + "\\"):   
            for file2 in files2: 
                files_to_delete.append(file2)
                
        
        df_new = df.loc[~df['File'].isin(files_to_delete)]
        
        df_new.to_csv(".\\" + label + ".csv", index=False)
                
        print(len(files_to_delete))