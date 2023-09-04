import os
import json
import csv

#data -> csv_files

folder= "data"


def multireplace(s, l, v):
    for c in l:
        s = s.replace(c,v)
    return s



# Get the list of all JSON files in the directory
json_files = os.listdir(folder)

# Create a new directory for the CSV files
csv_directory = "csv_files"
if os.path.exists(csv_directory):
    # Delete the directory if it exists
    for file in os.listdir(csv_directory):
        os.remove(os.path.join(csv_directory, file))
else:
    os.mkdir(csv_directory)

# Iterate over the JSON files
for json_file in json_files:
    print(json_file)
    # Load the file into a JSON object
    with open(os.path.join(folder,json_file), "r") as f:
        json_data = json.load(f)

    counts = []
    #Get max answer count
    for dict in json_data["ResultList"]:
        counts += [len(dict["Answers"])]
    
    #print("File max is", max(counts))
    keys=["Questions","Answers","Indexes"]
    keys_head=["Questions"] +["Answers"]+[""] * (max(counts)-1) + ["Indexes"]+[""] * (max(counts)-1)
    # Create a CSV file
    csv_file = os.path.join(csv_directory, json_file.replace(".json", ".csv"))
    with open(csv_file, "w") as f:
            writer = csv.writer(f, delimiter=';')
            writer.writerow(keys_head)
            for dict in json_data["ResultList"]:
                line = [dict["Questions"][0]]
                #Answers
                arg = dict["Answers"]
                for i in range(0,max(counts)):
                    if(i < len(arg)):
                        line+=[arg[i]]
                    else:
                        line+=[" "]
                #Indexes
                if("Indexes" in dict):
                    arg = dict["Indexes"]
                    for i in range(0,max(counts)):
                        if(i < len(arg)):
                            line+=[arg[i]]
                        else:
                            line+=[" "]
                print(line)
                
                writer.writerow(line)

#correct Umlaute


# Print a message to the user
print("All JSON files have been converted to CSV files.")

