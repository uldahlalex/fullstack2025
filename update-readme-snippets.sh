#!/bin/bash

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
        "sql")
            echo "sql"
            ;;
        "rb")
            echo "ruby"
            ;;
        "php")
            echo "php"
            ;;
        "go")
            echo "go"
            ;;
        "rs")
            echo "rust"
            ;;
        "ts")
            echo "typescript"
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
    local in_target_block=0
    local marker="[//]: # (FILE: $filename)"
    local skip_next_block=0
    local current_marker=""
    local language=$(get_language "$filename")
    
    # Check if file exists
    if [ ! -f "$filename" ]; then
        echo "Warning: File $filename does not exist!"
        return 1
    fi

    while IFS= read -r line || [ -n "$line" ]; do
        # Check if this is any file marker
        if [[ "$line" =~ \[//\]:\ \#\ \(FILE:* ]]; then
            current_marker="$line"
            if [ "$line" = "$marker" ]; then
                # Found our target marker - output it and the new code block
                echo "$line" >> "$temp_file"
                if [ -n "$language" ]; then
                    echo "\`\`\`$language" >> "$temp_file"
                else
                    echo "\`\`\`" >> "$temp_file"
                fi
                cat "$filename" >> "$temp_file"
                echo "" >> "$temp_file"  # Add newline before closing backticks
                echo "\`\`\`" >> "$temp_file"
                echo "" >> "$temp_file"  # Add newline after code block
                in_target_block=1
                skip_next_block=1
                continue
            else
                # This is a different marker - output it
                echo "$line" >> "$temp_file"
                in_target_block=0
                skip_next_block=0
            fi
            continue
        fi

        if [ $in_target_block -eq 1 ]; then
            if [ "$line" = '```' ]; then
                in_target_block=0
            fi
            continue
        fi

        if [ "$skip_next_block" = "1" ] && [ "$line" = '```' ]; then
            skip_next_block=0
            continue
        fi

        if [ "$skip_next_block" = "0" ]; then
            echo "$line" >> "$temp_file"
        fi
    done < "README.md"

    mv "$temp_file" "README.md"
}

# Main script
if [ ! -f "README.md" ]; then
    echo "Error: README.md not found!"
    exit 1
fi

# Extract all file references and process each one
grep -o '\[//\]: # (FILE: [^)]*)' README.md | while read -r marker; do
    # Extract filename from marker
    filename=$(echo "$marker" | sed 's/\[\/\/\]: # (FILE: \(.*\))/\1/')
    process_file_reference "$filename"
done