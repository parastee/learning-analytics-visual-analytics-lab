import pandas as pd
import json

filenames = [
    'unis-info',
    'unis-departments-info',
    'unis-programs-info',
]

for filename in filenames:
    filename = './data/' + filename
    filename_json = filename + '.json'
    filename_csv = filename + '.csv'
    with open(filename_json) as f:
        data = json.load(f)

    print(filename_json, "loaded")
    df = pd.json_normalize(data)

    df.to_csv(filename_csv, index=False, encoding='utf-8')
    print(filename_csv, "saved")

