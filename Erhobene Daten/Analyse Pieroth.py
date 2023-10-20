import os
import csvmethods
import json
import csv
from Json2Csv import ConvertAllJsonFiles

ConvertAllJsonFiles("pierothTests")
csv_folder = "pierothTests_csv"
csv_files = os.listdir(csv_folder)

geschmackAttribute = ["Süße","Säure","Bitterkeit"]
geschmackAns = ["wenig","mittel","viel"]
geschmackAbg = ["kurz","mittel","lang"]
csvmethods.NewOrClearFolder(csv_folder+"_correct")
questionnaire = {}  

for file in csv_files:

    with open(os.path.join(csv_folder,file),"r") as f:
        lines = f.readlines()
    f.close()
    header = lines[0]
    answer_count = int(csvmethods.countEmpties(header)/2+1)
    lines_new = []
    prefixline = ""

    index = 1
    for line in lines:
        if(len(line) > 1):
            line = csvmethods.correctUmlaute(line)
            line = csvmethods.trimSpace(line)

            if("Gesamteindruck" in line or "Geruch" in line or "Geschmack" in line):
                if(index<20):
                    line = "Weißwein " + line
                else:
                    line = "Rotwein " + line

            #print(index,"\t",line)        
            if(index == 13 or index == 23):
                line = line.split(";")
                for i in range(1,4):
                    if(len(line[i])>1):
                        line[i] = geschmackAttribute[i-1] + " " + line[i]
                line = ";".join(line)
            if(index == 15 or index == 25):
                line = line.split(";")
                line[1] = "Alkohol " + line[1]
                if(len(line[2])>1):
                    line[2] = "Abgang " + line[2]
                for i in range(3,answer_count+1):
                    line[i] = " "
                line = ";".join(line)


            line = ";".join(line.split(";")[:answer_count+1])
            line = prefixline + line
            prefixline = ""
            if(line.count(";")<answer_count):
                prefixline = line.replace("\n"," ")
            else:
                
                if("Proband" not in line and "Studie" not in line and "Umgebung" not in line):
                    lines_new += [line+"\n"]
                    #print(str(index)+"\t",line)    
        index+=1
        
    lines_new[-1] = lines_new[-1][:-1]  #letzte \n löschen
        
    #zwischenspeichern
    
    with open(os.path.join(csv_folder+"_correct",file.replace(".csv","_correct.csv")),"w") as f:  
        f.writelines(lines_new)
        f.close()

    #daten zusammentragen
    lines = [x for x in lines_new if x!= "\n"]
    for line in lines:
        question = line.split(";")[0]
        answers = [x.replace("\n","") for x in line.split(";")[1:] if (x!=" " and x!=" \n" and x!="\n" and x!="")]
        #print(question,"-",answers)

        if question not in questionnaire:
                questionnaire[question] = [["Total",1]]
        else:
            questionnaire[question][0][1] +=1

        for answer in answers:
            contains, i = csvmethods.QuestionHasAnswer(questionnaire[question], answer)
            if contains:
                questionnaire[question][i][1] += 1
            else:
                questionnaire[question] += [[answer,1]]

print("All csv files have been analysed")


lines = []

for question in questionnaire:
    questionnaire[question] = sorted(questionnaire[question], key=lambda x: x[0])
    #print(questionnaire[question])
    
    lines += question + "\n"
    for answer in questionnaire[question]:
        lines += str(answer[0]) + ";" + str(answer[1]) + "\n"
    lines += "\n"

with open (os.path.join("Auswertung", "pieroth" + ".csv"),"w") as f:
    f.writelines(lines)
    f.close()
        