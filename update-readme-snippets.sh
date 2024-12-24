#!/bin/bash

# Enable debugging
set -x

# Function to get language based on file extension
get_language() {
    local filename="$1"
    case "${filename##*.}" in
        sh) echo "bash" ;;
        js) echo "javascript" ;;
        py) echo "python" ;;
        cpp|cc|cxx) echo "cpp" ;;
        c) echo "c" ;;
        cs) echo "csharp" ;;
        java) echo "java" ;;
        *) echo "" ;;
    esac
}

# Function to process a file reference
process_file_reference() {
    local filename="$1"
    local temp_file=$(mktemp)
    local marker="[//]: # (FILE: $filename)"
    local language=$(get_language "$filename")
    local in_code_block=false
    
    echo "Processing file: $filename"
    
    if [ ! -f "$filename" ]; then
        echo "Warning: File $filename does not exist!"
        return 1
    fi

    while IFS= read -r line || [ -n "$line" ]; do
        # If we find our marker, insert the new content
        if [[ "$line" == "$marker" ]]; then
            echo "Found marker for $filename"
            echo "$line" >> "$temp_file"
            if [ -n "$language" ]; then
                echo "\`\`\`$language" >> "$temp_file"
            else
                echo "\`\`\`" >> "$temp_file"
            fi
            cat "$filename" >> "$temp_file"
            echo "\`\`\`" >> "$temp_file"
            echo "" >> "$temp_file"
            in_code_block=true
            continue
        fi
        
        # If we're in a code block from our marker, skip lines until we find another marker
        if $in_code_block; then
            if [[ "$line" =~ ^\[\/\/\]:\ \#\ \(FILE: ]]; then
                in_code_block=false
                echo "$line" >> "$temp_file"
            fi
            continue
        fi
        
        # Otherwise, copy the line as-is
        echo "$line" >> "$temp_file"
    done < "README.md"

    mv "$temp_file" "README.md"
}

# Main script
echo "Script starting..."

if [ ! -f "README.md" ]; then
    echo "Error: README.md not found!"
    exit 1
fi

echo "Initial README.md contents:"
cat "README.md"

# Process all file references
echo "Finding file references..."
grep -o '\[//]: # (FILE: [^)]*)' README.md | while read -r marker; do
    filename=$(echo "$marker" | sed 's/\[\/\/\]: # (FILE: \(.*\))/\1/')
    echo "Processing marker: $marker"
    echo "Extracted filename: $filename"
    process_file_reference "$filename"
done

echo "Script completed"
echo "Final README.md contents:"
cat "README.md"