#!/bin/bash

# Define the list of files you want to include in the README.md
declare -a files=("./server/Infrastructure.Postgres.Scaffolding/scaffold.sh")

# Function to insert the content of a file into the README.md
insert_file_contents() {
    local filename="$1"
    local marker="[//]: # (The code block below should insert the raw file contents from $filename )"
    local temp_file=$(mktemp)

    # Print everything before the marker and the following code block
    awk -v marker="$marker" 'BEGIN {print_block=1} $0 ~ marker {print_block=0} print_block' README.md > "$temp_file"

    # Print the marker
    echo "$marker" >> "$temp_file"

    # Print the file contents
    echo '```' >> "$temp_file"
    cat "$filename" >> "$temp_file"
    echo '```' >> "$temp_file"

    # Print everything after the marker and the following code block
    awk -v marker="$marker" 'BEGIN {print_block=1} $0 ~ marker {print_block=0} print_block || $0 ~ "```"' README.md >> "$temp_file"

    # Replace the README.md with the temp file
    mv "$temp_file" README.md
}

# Update the README.md with the contents of each file
for filename in "${files[@]}"; do
    insert_file_contents "$filename"
done