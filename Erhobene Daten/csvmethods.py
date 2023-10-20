import os


def NewOrClearFolder(folder):
    #make new dir
    if os.path.exists(folder):
        # Delete the directory if it exists
        for file in os.listdir(folder):
            os.remove(os.path.join(folder, file))
    else:
        os.mkdir(folder)

umlaute = { 
    '"':'',
    "Ã¤":"ä",
    "Ã¼":"ü",
    "Ã¶":"ö",
    "ÃŸ":"ß",
    "Ãœ":"Ü",
    "<color=red>":"",
    "</color>":"",
    " zu.":"",
    " <b>":"",
    "<b>":"",
    "</b>":"",
    "bläulichen":"bläul.",
    "laktisch":"Laktisch",
    '"Buttrig/Laktisch"':"ButtrigLaktisch",
    '"Schmelz/Cremig"':"SchmelzCremig",
    "süße":"Süße",
    "17 or less":"17 oder jünger",
    "Buttrig/Laktisch":"ButtrigLaktisch",
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
    "butter": "ButtrigLaktisch",
    "lactic":"",
    "butter": "SchmelzCremig",
    "cream":"",
    "Bitte ordnen Sie der ":"",
    " die entsprechende Farbe":" Farbe",
    "Bitte ordnen Sie derselben ":"",
    " den entsprechenden":"",
    "Mehrfachnennungen sind möglich!":"",
    "Please pick a fitting color for sample ":"Probe ",
    "Age":"Alter",
    "Gender":"Geschlecht",
    "Handiness":"Händigkeit",
    "Please pick a fitting ":"Probe ",
    "for the sample.":"",
    "for the same sample.":"",
    "You can choose multiple answers!":"",
    "aroma ":"Geruch",
    "taste ":"Geschmack",
    "world?":"world?",
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
    "1: Überhaupt nicht -7: Vollständig":"",
    ".Eine Auswahl pro Zeile.":"",
    "Bitte bewerten Sie den ":"",
    "Bitte wählen Sie die passende ":"",
    "Angabge":"Angabe",
    "keineAngabe":"keine Angabe",
    "Bitte wählen Sie bis zu 4 Attribute zum Geruch aus":"Geruch",
    "Farbe.":"Farbe",
    "Geruch.":"Geruch"
    
    

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
    s = s.replace(" ;",";").replace("; ",";").replace(" \n","\n")
    
    return s

def CheckIfIdIsAccepcted(filename):
    accepted_ids =["001","002","004","006","007","008","009","010","011","014","016","017","018","019","020","021","022","024","025","026"]
    for id in accepted_ids:
        if id in filename:
            return True
    return False

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

