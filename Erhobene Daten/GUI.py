import tkinter as tk
import csv
import json
import os
# Create the main window
root = tk.Tk()

# Create the listbox
listbox = tk.Listbox(root)
listbox.pack()

# Add the JSON files to the listbox
for json_file in os.listdir("./data"):
    if json_file.endswith(".json"):
        listbox.insert(tk.END, json_file)

# Define the export_to_csv function
def export_to_csv():
    # Get the selected JSON file
    json_file = listbox.get(tk.ACTIVE)

    # Open the JSON file
    with open(json_file, "r") as f:
        json_data = json.load(f)

    # Create a CSV file
    with open(json_file.replace(".json", ".csv"), "w") as f:
        writer = csv.writer(f)
        writer.writerow(list(json_data.keys()))
        writer.writerows(list(json_data.values()))
        
# Create the button
button = tk.Button(root, text="Export to CSV", command=export_to_csv)
button.pack()



# Run the main loop
root.mainloop()
