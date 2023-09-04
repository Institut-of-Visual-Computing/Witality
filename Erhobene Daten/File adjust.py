import os
import json
import csv

#csv files -> csv files correct

# Functions

umlaute = { 
    '"':'',
    "Ã¤":"ä",
    "Ã¼":"ü",
    "Ã¶":"ö",
    "ÃŸ":"ß",
    "Ãœ":"Ü",
    "<color=red>":"",
    "</color>":"",
    " zu.\n":"",
    "Schmelz\n":"Schmelz",
    "Buttrig\n":"Buttrig",
    " <b>":"",
    "<b>":"",
    "</b>":"",
    "anwesend. \n":"anwesend.",
    "Umgebung. \n":"Umgebung.",
    "gezogen. \n":"gezogen.",
    "Umgebung? \n":"Umgebung?",
    "Personen? \n":"Personen?",
    "ausgeruht. \n":"ausgeruht.",
    "bläulichen":"bläul.",
    "laktisch":"Laktisch",
    '"Buttrig/Laktisch"':"ButtrigLaktisch",
    '"Schmelz/Cremig"':"SchmelzCremig",
    "süße":"Süße",
    "17 or less":"17 oder jünger",
    "ButtrigLaktisch":"Buttrig/Laktisch",
    "Done.":"Verstanden.",
    "Female":"Weiblich",
    "Male":"Männlich",
    "left-handed":"Linkshändig",
    "right-handed":"Rechtshändig",
    "Daily":"Täglich",
    "Weekly":"Wöchentlich",
    "Monthly":"Einige Male im Monat",
    "Just a few times":"Weniger oft",
    "This is my first time":"Dies ist mein erstes Mal",
    "pale red with brown rim": "Hellrot mit braunem Rand",
    "ruby": "Rubinrot",
    "deep red with bluish reflection": "Dunkelrot mit bläul. Reflexen",
    "fruity": "Fruchtig",
    "strawberry": "Erdbeere",
    "cherry": "Kirsche",
    "dark berries": "Dunkle Beeren",
    "plum": "Pflaume",
    "vegetal": "Vegetativ",
    "spicy": "Würzig",
    "clove": "Nelke",
    "vanilla": "Vanille",
    "pepper": "Pfeffer",
    "smoke": "Rauchig",
    "wood": "Holz",
    "dull": "Dumpf",
    "low sweetness": "Leichte Süße",
    "medium sweetness": "Süß",
    "high sweetness": "Starke Süße",
    "low acidity": "Dezente Säure",
    "medium acidity": "Sauer",
    "high acidity": "Dominante Säure",
    "bitter": "Bitter",
    "high alcohol": "Alkoholisch",
    "astringent": "Adstringierend",
    "tannin": "Tannine",
    "wood": "Holz",
    "pale yellow": "Hellgelb",
    "medium yellow": "Mittleres gelb",
    "yellow-green": "Gelblich-grün",
    "floral": "Blumig",
    "rose": "Rose",
    "fruity": "Fruchtig",
    "lemon": "Zitrus",
    "honeydew": "Honigmelone",
    "peach": "Pfirsich",
    "apple": "Apfel",
    "pear": "Birne",
    "vegetal": "Vegetativ",
    "butter\n": "ButtrigLaktisch",
    "lactic":"",
    "butter\n": "SchmelzCremig",
    "cream":"",
    "Bitte ordnen Sie der ":"",
    " die entsprechende Farbe zu.":" Farbe",
    "Bitte ordnen Sie derselben ":"",
    " den entsprechenden":"",
    "Mehrfachnennungen sind möglich!":"",
    "Please pick a fitting color for sample ":"Probe ",
    "Age":"Alter",
    "Gender":"Geschlecht",
    "Handiness":"Händigkeit",
    "Please pick a fitting ":"Probe ",
    "for the sample.\n":"",
    "for the same sample.\n":"",
    "You can choose multiple answers!":"",
    "aroma ":"Geruch",
    "taste ":"Geschmack",
    "world?\n":"world?",
    "Environment. \n":"Environment.",
    "people)\n":"people)",
    "surroundings. \n":"surroundings.",
    "world. \n":"world.",
    "world? \n":"world?",
    "Personen)? \n":"Personen)?",
    "rested.\n":"rested.",
    "sweet":"Süß",
    "acidic":"Sauer",
    "Take a look at your environment and make yourself comfortable with the room.":"Umschauen",
    "Take a look at your environment before answering the following questions.":"Umschauen",
    "Schauen Sie sich im virtuellen Raum einmal um und finden Sie sich im Raum ein.":"Umschauen",
    "Schauen Sie sich im virtuellen Raum einmal um, um die folgenden Fragen zu beantworten.":"Umschauen",
    "I'm feeling fresh and rested.":"Ich fühle mich frisch und ausgeruht.",
    "How often do you use virtual reality (VR)-Headsets?":"Wie oft benutzen Sie Virtual Reality (VR)-Headsets?",
    "I'm feeling present in the virtual Environment.":"Ich fühlte mich im virtuellen Raum anwesend.",
    "How much were you aware of the real world, while experiencing the virtual world?":"Wie bewußt war Ihnen die reale Welt, während Sie sich durch die virtuelle Welt bewegten (z.B. Geräusche, andere Personen)?",
    "I still paid attention to the real surroundings.":"Ich achtete noch auf die reale Umgebung.",
    "My attention was totally drawn into the virtual world.":"Meine Aufmerksamkeit war von der virtuellen Welt völlig in Bann gezogen.",
    "How much did your virtual experience equaled a real world?":"Wie sehr glich Ihr Erleben der virtuellen Umgebung dem Erleben einer realen Umgebung?",
    "How much did you virtual experience equaled a real world?":"Wie sehr glich Ihr Erleben der virtuellen Umgebung dem Erleben einer realen Umgebung?",
    "1: Strongly disagree -7: Totally agree":"",
    "1: Strongly disagree. -7: Totally agree.":"",
    "(i.e. Sounds, other people)":"",
    "1: Strongly aware -7: Totally unaware":"",
    "1: Not equal at all -7: Completely equal":"",
    "1: Trifft gar nicht zu -7: Trifft völlig zu":"",
    "1: Trifft gar nicht zu. -7: Trifft völlig zu.":"",
    "1: Extrem bewußt -7: Unbewußt":"",
    "1: Überhaupt nicht -7: Vollständig":""

}

def correctUmlaute(s):
    a = s
    for key in umlaute:
        a = a.replace(key,umlaute[key])

    if(a == s):
        return a
    else:
        return correctUmlaute(a)

def trimSpace(s):
    if len(s) < 1:
        return s
    if s[0] == " ":
        return trimSpace(s[1:])
    if s[-1] == " ":
        return trimSpace(s[:-1])
    s = s.replace(" ;",";").replace("; ",";")
    while(";;" in s):
        s = s.replace(";;","; ;")

    return s



# Main
folder = "csv_files"

# Get all CSV Files
files = os.listdir(folder)

# Create a new directory for the CSV files
csv_directory = "csv_files_correct"
if os.path.exists(csv_directory):
    # Delete the directory if it exists
    for file in os.listdir(csv_directory):
        os.remove(os.path.join(csv_directory, file))
else:
    os.mkdir(csv_directory)

# Iterate over csv
for file in files:
    with open(os.path.join(folder,file),"r") as f:
        lines = f.readlines()
        environment = lines[-2].split(";")[1].replace("\n","")
        lines_new = []
        for line in lines:
            if(len(line) > 1):
                line = correctUmlaute(line)
                if(line[:5] == "Probe"):
                    if(line[6:8].isnumeric()):
                        lastSample = line[6:9]
                        if(line[9] == "."):
                            line = line[:9] + " Farbe" + line[10:]
                    if(line[6] == "G"):
                        line = line[:6] + lastSample + line[5:]
                    line = environment + " " + line
                line = trimSpace(line)
                lines_new += [line]
                #print("Line",len(line),line)
        f.close()


        #create new file
        file_new = os.path.join(csv_directory, file.replace(".csv","_correct.csv"))
        with open (file_new, "w") as f_new:
            f_new.writelines(lines_new)
            f_new.close()

print("All files have been corrected.")