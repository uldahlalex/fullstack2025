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
    local in_code_block=0
    local marker="[//]: # (FILE: $filename)"
    local skip_next_block=0
    local language=$(get_language "$filename")
    
    echo "Processing file: $filename"
    echo "Language detected: $language"
    
    # Check if file exists
    if [ ! -f "$filename" ]; then
        echo "Warning: File $filename does not exist!"
        echo "Current directory: $(pwd)"
        echo "Directory contents:"
        ls -la
        return 1
    fi

    echo "File content to be inserted:"
    cat "$filename"
    
    local processing_marker=0
    while IFS= read -r line || [ -n "$line" ]; do
        # Debug output
        echo "Processing line: $line"
        echo "in_code_block: $in_code_block"
        echo "skip_next_block: $skip_next_block"
        echo "processing_marker: $processing_marker"

        if [[ "$line" == "[//]: # (FILE:"* ]]; then
            if [[ "$line" == "$marker" ]]; then
                echo "Found target marker: $line"
                echo "$line" >> "$temp_file"
                if [ -n "$language" ]; then
                    echo "\`\`\`$language" >> "$temp_file"
                else
                    echo "\`\`\`" >> "$temp_file"
                fi
                cat "$filename" >> "$temp_file"
                echo "" >> "$temp_file"
                echo "\`\`\`" >> "$temp_file"
                echo "" >> "$temp_file"
                in_code_block=1
                skip_next_block=1
                processing_marker=1
                continue
            else
                echo "Found different marker: $line"
                processing_marker=0
                in_code_block=0
                skip_next_block=0
                echo "$line" >> "$temp_file"
            fi
            continue
        fi

        if [ $in_code_block -eq 1 ]; then
            if [[ "$line" == "\`\`\`"* ]]; then
                echo "End of code block found"
                in_code_block=0
            fi
            continue
        fi

        if [ "$skip_next_block" = "1" ] && [[ "$line" == "\`\`\`"* ]]; then
            echo "Skipping existing code block"
            skip_next_block=0
            continue
        fi

        if [ "$skip_next_block" = "0" ]; then
            echo "$line" >> "$temp_file"
        fi
    done < "README.md"

    echo "Temporary file contents:"
    cat "$temp_file"
    
    mv "$temp_file" "README.md"
    echo "Updated README.md contents:"
    cat "README.md"
}

# Main script
echo "Script starting..."
echo "Current directory: $(pwd)"
echo "Directory contents:"
ls -la

if [ ! -f "README.md" ]; then
    echo "Error: README.md not found!"
    exit 1
fi

echo "Initial README.md contents:"
cat "README.md"

echo "Finding file references..."
grep -o '\[//\]: # (FILE: [^)]*)' README.md | while read -r marker; do
    echo "Found marker: $marker"
    filename=$(echo "$marker" | sed 's/\[\/\/\]: # (FILE: \(.*\))/\1/')
    echo "Extracted filename: $filename"
    process_file_reference "$filename"
done

echo "Script completed"
echo "Final README.md contents:"
cat "README.md"