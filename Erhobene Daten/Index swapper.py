import os

def countEmpties(line):
    c = 0
    for i in line.split(";"):
        if(i == "" or i == "\n" or i == " "):
            c+=1
    return c

def splitAnswerIndex(a, c):
    b = []
    
    for i in range(0,c):
        if(len(a) <= c):
            b+=[[a[i]," "]]
        else:
            b+=[[a[i], a[i+c]]]
    return b

def QuestionHasAnswer(q,a):
    for i in range(0,len(q)):
        (x,y) = q[i]
        if(x == a):
            return True, i
    return False, -1


folder = "csv_files_correct"

files = os.listdir(folder)

questions = {}
# Create a new directory for the Log files
logs = "Logs"
if os.path.exists(logs):
    # Delete the directory if it exists
    for file in os.listdir(logs):
        os.remove(os.path.join(logs, file))
else:
    os.mkdir(logs)

# Iterate over logs
for file in files:
    with open(os.path.join(folder,file),"r") as f:
        # Amount of Answers
        header = f.readline()
        
        answer_count = int(countEmpties(header)/2 + 1)
        lines = f.readlines()
        f.close()
        

        for line in lines:
            question = line.split(";")[0]
            answers = splitAnswerIndex(line.split(";")[1:], answer_count)
            if(len(answers) == answer_count):    #Only lines where question is asked. Not Proband ID etc
                if question not in questions:
                    questions[question] = []
                
                for (answer,index) in answers:
                    index = index.replace("\n","")
                    if(answer != " "):
                        contains, i = QuestionHasAnswer(questions[question], answer)
                        if contains:
                            if index not in questions[question][i][1]:
                                questions[question][i][1] += [index]
                        else:
                            questions[question] += [[answer, [index]]]

#---------write files
for file in files: 
    with open(os.path.join(folder,file),"r") as f:
        header = f.readline()
        answer_count = int(countEmpties(header)/2 + 1)
        lines = f.readlines()
        f.close()
        lines_new = [header]

        for line in lines:
            question = line.split(";")[0]
            answers = splitAnswerIndex(line.split(";")[1:], answer_count)
            sep = ";"
            line = question
            for answer,index in answers:
                line += sep + answer


            lines_new += line + "\n"

    #create new file
    file_new = os.path.join("Logs", file.replace("_correct.csv",".csv"))
    with open (file_new, "w") as f_new:
        f_new.writelines(lines_new)
        f_new.close()


#testing

for k in sorted(questions.keys()):
    print(k)
    
        

