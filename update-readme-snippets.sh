#!/bin/bash

# Enable debugging
set -x

# Function to get language based on file extension
get_language() {
    local filename="$1"
    local extension="${filename##*.}"
    case "$extension" in
        "sh")
            echo "bash"
            ;;
        "js")
            echo "javascript"
            ;;
        "py")
            echo "python"
            ;;
        "cpp"|"cc"|"cxx")
            echo "cpp"
            ;;
        "c")
            echo "c"
            ;;
        "cs")
            echo "csharp"
            ;;
        "java")
            echo "java"
            ;;
        *)
            echo ""
            ;;
    esac
}

# Function to process a single file reference and its code block
process_file_reference() {
    local filename="$1"
    local temp_file=$(mktemp)
    local marker="[//]: # (FILE: $filename)"
    local language=$(get_language "$filename")
    
    echo "Processing file: $filename"
    echo "Language detected: $language"
    
    if [ ! -f "$filename" ]; then
        echo "Warning: File $filename does not exist!"
        return 1
    fi

    # Read file line by line
    while IFS= read -r line || [ -n "$line" ]; do
        if [ "$line" = "$marker" ]; then
            # Found our marker, write it and the new content
            echo "$line" >> "$temp_file"
            if [ -n "$language" ]; then
                echo ""\`\`\`$language"" >> "$temp_file"
            else
                echo ""\`\`\`"" >> "$temp_file"
            fi
            cat "$filename" >> "$temp_file"
            echo "" >> "$temp_file"
            echo ""\`\`\`"" >> "$temp_file"
            echo "" >> "$temp_file"
            
            # Skip lines until we find the next marker or end of file
            while IFS= read -r skip_line; do
                if [[ "$skip_line" =~ ^\[\/\/\]:\ \#\ \(FILE: ]]; then
                    echo "$skip_line" >> "$temp_file"
                    break
                fi
            done
        else
            echo "$line" >> "$temp_file"
        fi
    done < "README.md"

    # Replace the original file with our new version
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

# Find all file references and process them
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