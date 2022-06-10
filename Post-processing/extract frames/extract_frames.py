from genericpath import exists
from os import walk
from pathlib import Path
import os
import cv2

# Constants
PATH = '.'
OUTPUT_PATH = './data'
FILE_FORMATS = ['.h265', '.h264']

def extract_video_frames(file_path, output_path):
  
  Path(output_path).mkdir(parents=True, exist_ok=True)

  file_details = file_path.split('\\')[1:]

  # Subject
  subject = file_details[0].lower().replace(' ', '')
  
  # Scenario
  scenario = file_details[1].lower().replace(' ', '')
    
  # Frame number    
  vidcap = cv2.VideoCapture(file_path)
  success, image = vidcap.read()
  count = 0
  while success:
    
    output_file_name = output_path + '/' + subject + '_' + scenario + '_' + "frame%d.jpg" % count
    
    if not exists(output_file_name):
    
      cv2.imwrite(output_path + '/' + subject + '_' + scenario + '_' + "frame%d.jpg" % count, cv2.rotate(image, cv2.ROTATE_90_COUNTERCLOCKWISE))          
      
      success, image = vidcap.read()
          
      print(output_path + '/' + subject + '_' + scenario + '_' + "frame%d.jpg  [" % count, success, ']')
    
    else: 
      print(output_path + '/' + subject + '_' + scenario + '_' + "frame%d.jpg  exists, skipping" % count)
    
    count += 1
    

def convert_to_mp4(file_path, output_path):
      
  Path(output_path).mkdir(parents=True, exist_ok=True)
    
  file_details = file_path.split('\\')[1:]

  # Subject
  subject = file_details[0].lower().replace(' ', '')
  
  # Scenario
  scenario = file_details[1].lower().replace(' ', '')
    
  new_file_name = subject + '_' + scenario + '_' +  Path(file).stem +'.mp4';
  os.system("ffmpeg -hide_banner -loglevel error -framerate 30 -n -i \"" + os.path.join(file_path) +"\" -vf \"transpose=dir=2\" \"" + os.path.join(output_path, new_file_name) + "\"")


for (path, directories, files) in walk(PATH):
  for file in files:
      
      # Convert to .mp4
      if file.lower().endswith(('.h265', '.h264')):
            
        print('Starting ', os.path.join(path, file) + '...')
        
        convert_to_mp4(os.path.join(path, file), './videos')   
        
        extract_video_frames(os.path.join(path, file), OUTPUT_PATH + '/' + Path(file).stem)   
  