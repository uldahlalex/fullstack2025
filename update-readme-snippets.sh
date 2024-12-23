#!/bin/bash

# Get list of file paths from README.md comments
files=$(grep -oP '(?<=FILE: ).*(?=\))' README.md)

# Function to insert the content of a file into the README.md
insert_file_contents() {
    local filename="$1"
    local marker="[//]: # (FILE: $filename)"
    local temp_file=$(mktemp)

    # Print everything up to and including the marker
    awk -v marker="$marker" 'BEGIN {print_block=1} $0 ~ marker {print_block=0; print} print_block' README.md > "$temp_file"

    # Print the file contents
    echo '```' >> "$temp_file"
    cat "$filename" >> "$temp_file"
    echo '```' >> "$temp_file"

    # Print everything after the marker, skipping the old code block if it exists
    awk -v marker="$marker" 'BEGIN {print_block=1} $0 ~ marker {getline; getline; getline; print_block=2} print_block == 2' README.md >> "$temp_file"

    # Replace the README.md with the temp file
    mv "$temp_file" README.md
}

# Update the README.md with the contents of each file
for filename in $files; do
    if [ -f "$filename" ]; then
        insert_file_contents "$filename"
    else
        echo "File $filename does not exist!"
    fi
done