import os
import json

def count_keywords_in_folder(folder_path, keywords):
    keyword_counts = {}

    # Iterate over files in the folder
    for filename in os.listdir(folder_path):
        file_path = os.path.join(folder_path, filename)

        # Check if the file is a JSON file
        if filename.endswith('.json') and os.path.isfile(file_path):
            with open(file_path, 'r') as json_file:
                try:
                    data = json.load(json_file)

                    # Count keywords in the JSON data
                    count_keywords(data, keyword_counts, keywords)
                except json.JSONDecodeError:
                    print(f"Error decoding JSON file: {file_path}")

    return keyword_counts


def count_keywords(data, keyword_counts, keywords):
    if isinstance(data, dict):
        for value in data.values():
            count_keywords(value, keyword_counts, keywords)
    elif isinstance(data, list):
        for item in data:
            count_keywords(item, keyword_counts, keywords)
    elif isinstance(data, str):
        for keyword in keywords:
            if keyword in data:
                keyword_counts[keyword] = keyword_counts.get(keyword, 0) + 1


# Get the current working directory
current_dir = os.getcwd()

# Specify the folder name (directory) within the current working directory containing JSON files
folder_name = 'Logs'

# Create the full path by joining the current directory and the folder name
folder_path = os.path.join(current_dir, folder_name)

# Specify the keywords to count
keywords = ['Weinkeller', 'Sensoriklabor', 'CATA','Weißwein','Rotwein','131','364','720','597','983','644','322']

# Call the function to count keywords in the folder
result = count_keywords_in_folder(folder_path, keywords)

# Print the keyword counts
for keyword, count in result.items():
    print(f"{keyword} occurred {count} times.")

# Find the lowest count among keywords
lowest_count = min(result.values())

# Find the keywords with the lowest count
keywords_with_lowest_count = [keyword for keyword, count in result.items() if count == lowest_count]

# Print the keyword(s) with the lowest count
if keywords_with_lowest_count:
    if len(keywords_with_lowest_count) == 1:
        print(f"{keywords_with_lowest_count[0]} had the lowest count.")
    else:
        keywords_str = ', '.join(keywords_with_lowest_count)
        print(f"{keywords_str} had the lowest count.")
else:
    print("No keywords found.")
