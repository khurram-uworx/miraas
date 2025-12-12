# Issue Reports

This directory contains user-reported issues with inheritance calculations.

## File Format

Each issue is saved as a numbered text file (e.g., `0001.txt`, `0002.txt`) containing:

1. **Header**: Issue number and timestamp
2. **User Comment**: Description of the problem
3. **Calculation Data**: Human-readable summary of the input
4. **Raw JSON**: Complete serialized request for debugging

## Usage

When users encounter unexpected calculation results, they can:

1. Click "Report Issue with Calculation" button
2. Describe what they expected vs. what they got
3. Submit the report

The system automatically:
- Assigns a sequential issue number
- Captures the complete calculation request
- Saves everything to a timestamped file

## Developer Notes

- Files are created automatically by `IssueReportingService`
- Issue numbers are auto-incremented based on existing files
- All user input is sanitized and validated
- Maximum comment length: 2000 characters
- Minimum comment length: 10 characters