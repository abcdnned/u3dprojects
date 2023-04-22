#!/bin/bash

# Check if all input variables are present
if [ "$#" -ne 3 ]; then
    echo "Usage: $0 RotaterFilePath UtilsFolderPath QuaternionRotaterFilePath"
    exit 1
fi

rotater_file=$(find IKMan/Assets -name $1.cs -type f)

if [[ -z "$rotater_file" ]]; then
  echo "Error: Could not find $1 file in the current folder."
  exit 1
fi

utils_folder=$(find IKMan/Assets -name $2 -type d)

if [[ -z "$utils_folder" ]]; then
  echo "Error: Could not find Utils folder in the current folder."
  exit 1
fi

cp "$rotater_file" "$utils_folder/$3.cs"

sed -i '' 's/class '$1'/class '$3'/g' "$utils_folder/$3.cs"
sed -i '' 's/public '$1'/public '$3'/g' "$utils_folder/$3.cs"

echo "$1.cs has been moved to $utils_folder/$3.cs and all instances of '$1' have been replaced with '$3' in the file."

