#!/bin/bash

# Get list of file paths from README.md comments
files=$(grep -oP '(?<=FILE: ).*(?=\))' README.md)

# Function to replace the content of a file into the README.md
replace_file_contents() {
    local filename="$1"
    local marker="[//]: # (FILE: $filename)"
    local temp_file=$(mktemp)

    # Print everything before the marker
    awk -v marker="$marker" '$0 ~ marker {exit} {print}' README.md > "$temp_file"

    # Print the marker
    echo "$marker" >> "$temp_file"

    # Print the file contents
    echo '```' >> "$temp_file"
    cat "$filename" >> "$temp_file"
    echo '```' >> "$temp_file"

    # Print everything after the marker and the following code block
    awk -v marker="$marker" 'BEGIN {print_block=1} $0 ~ marker {getline; getline; getline; print_block=2} print_block == 2' README.md >> "$temp_file"

    # Replace the README.md with the temp file
    mv "$temp_file" README.md
}

# Remove existing marker comments from README.md
for filename in $files; do
    escaped_filename=$(printf '%s\n' "$filename" | sed -e 's/[\/&]/\\&/g')
    sed -i "/(FILE: $escaped_filename)/d" README.md
done

# Replace the existing code block in the README.md with the contents of each file
for filename in $files; do
    if [ -f "$filename" ]; then
        replace_file_contents "$filename"
    else
        echo "File $filename does not exist!"
    fi
done