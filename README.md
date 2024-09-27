# Resource Shortage Management System

## Key Considerations

- **Data File Size**: It is presumed that the shortages.json file remains manageable in size. At present, the entire content of this file is loaded into memory, and it is rewritten each time a request is added or removed.
- **Administrator Access**: The system is designed for use by a single administrator with the username `"admin"`.
- **Title Symbol**: Special symbols (like `-#!\|,.^`) in titles are ignored, as they are utilized to create composite objects that serve as keys in the shortages dictionary.

## System Requirements
  - .NET 8 SDK must be installed on your machine.
