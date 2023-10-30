import os
import csvmethods
from Json2Csv import ConvertAllJsonFiles

ConvertAllJsonFiles("cataTests")
csv_folder = "cataTests_csv"
csv_files = os.listdir(csv_folder)

csvmethods.NewOrClearFolder(csv_folder+"_correct")

cata_proband = {}
cata_w = {}   #weiß 
cata_r = {}   #rot 

for file in csv_files:

    #check if the id is accepted (participant was in all 3 environments)
    if(csvmethods.CheckIfIdIsAccepcted(file)):       
        with open(os.path.join(csv_folder,file),"r") as f:
            
            lines = f.readlines()
            header = lines[0]
            answer_count = int(csvmethods.countEmpties(header)/2+1)
            environment = lines[-2].split(";")[1].replace("\n","")
            probandID = lines[-8].split(";")[1].replace("\n","")
            environment = csvmethods.GetEnvironment(str(int(probandID)),file[:10])
            lines_new = []
            prefixline = ""
            if "CATA-W" in file:
                questionnaire = cata_w
            if "CATA-R" in file:
                questionnaire = cata_r


            for line in lines:
                if(len(line) > 1):
                    line = csvmethods.correctUmlaute(line)
                    if(line[:5] == "Probe"):
                        if(line[6:8].isnumeric()):
                            lastSample = line[6:9]
                            if(line[9] == "."):
                                line = line[:9] + " Farbe" + line[10:]
                        if(line[6] == "G"):
                            line = line[:6] + lastSample + line[5:]
                    line = environment + " " + line
                    line = csvmethods.trimSpace(line)
                    line = ";".join(line.split(";")[:answer_count+1])
                    line = prefixline + line
                    prefixline = ""
                    if(line.count(";")<answer_count):
                        prefixline = line
                    else:
                        if("Proband" not in line and "Studie" not in line and "Umgebung" not in line):
                            lines_new += [line+"\n"]
                
                
            f.close()
        lines_new[-1] = lines_new[-1][:-1]  #letzte \n löschen
        if (probandID in cata_proband):
            cata_proband[probandID] += ["\n\n"] + lines_new
        else:
            cata_proband[probandID] = lines_new
        #zwischenspeichern
    
        with open(os.path.join(csv_folder+"_correct",file.replace(".csv","_correct.csv")),"w") as f:  
            f.writelines(lines_new)
            f.close()

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

questionnaires = [cata_w, cata_r]
questionnaire_names= ["cata_w", "cata_r"]

for questionnaire in questionnaires:
    lines = []

    for question in questionnaire:
        questionnaire[question] = sorted(questionnaire[question], key=lambda x: x[0])
        #print(questionnaire[question])
        
        lines += question + "\n"
        for answer in questionnaire[question]:
            lines += str(answer[0]) + ";" + str(answer[1]) + "\n"
        lines += "\n"

    with open (os.path.join("Auswertung", "cata_" +("weiß" if questionnaire == cata_w else "rot") + ".csv"),"w") as f:
        f.writelines(lines)
        f.close()


lines = []
for proband in cata_proband:
    lines+=["Proband "+ '{:03d}'.format(int(proband)) + "\n"]
    lines+= cata_proband[proband]+["\n\n\n\n\n\n\n"]

with open (os.path.join("Auswertung","cata_pro_proband.csv"),"w") as f:
    f.writelines(lines)
    f.close()
