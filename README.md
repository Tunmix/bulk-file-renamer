# Bulk File Renamer

**Bulk File Renamer** is a simple and powerful tool that allows users to quickly rename multiple files in a folder with a customizable pattern. Ideal for organizing large batches of files, this app makes it easy to add prefixes, suffixes, and numbered sequences to file names.

## Project Background

This application was born out of a real need - I originally created a batch file version when I needed to rename multiple files for a project. As a game developer who hadn't previously created Windows desktop applications, I decided to transform it into a proper UI tool that anyone could use without needing to understand command-line operations.

The entire application was developed in a single night, which demonstrates how focused development with the right tools can be incredibly productive. The development process was assisted by Claude AI to help navigate the Windows Forms development landscape, making this an interesting experiment in AI-assisted coding. While the code and UI reflect a genuine collaboration (certainly not all AI-generated!), I learned more about Windows application development in that one night than I might have learned in a week through conventional methods.

The application is intentionally open-source because I believe in sharing practical tools that solve real problems. While the current version focuses on the core functionality of bulk renaming, there's potential for many additional features like file filtering, individual file selection via checkboxes, and more specialized renaming patterns. However, even in its current form, it handles the primary task of bulk file renaming perfectly.

## Features
- **Drag-and-Drop Folder Support**: Simply drag and drop a folder onto the app window to select your files.
- **Enhanced Visual Feedback**: Clear visual indicators when dragging folders over the application.
- **Customizable Prefix**: Choose a starting name for your files, such as `Image_`, `Document_`, etc.
- **Suffix Support**: Add a customizable suffix after the prefix name followed by the numbering.
- **Skip Hidden/System Files**: Automatically exclude hidden or system files from the renaming process.
- **Leading Zeros**: Add leading zeros to the file numbers to maintain proper sequence (useful for sorting files in order).
- **Keep or Remove Extensions**: Option to maintain or remove file extensions during renaming.
- **Live Preview**: Preview file names before applying the changes to ensure everything is correct.
- **Theme Support**: Choose between Light, Dark, or System theme.
- **Simple, Clean UI**: Intuitive interface for both novice and experienced users.

## How to Use

1. **Select a Folder**: Drag and drop a folder, or use the browse button to select a folder containing files you want to rename.
2. **Set the Prefix**: Enter a starting name for your files (e.g., `Photo`, `File`). Can be empty.
3. **Choose a Suffix**: Set a suffix for personal customized needs (e.g., `_edited`, `_final`).
4. **Toggle Optional Features**:
    - **Skip Hidden/System Files**: Exclude hidden or system files from renaming.
    - **Leading Zeros**: Enable or disable leading zeros for numbered files.
    - **Keep File's Extension**: Choose whether to keep or remove file extensions.
5. **Preview**: Check the preview list of how the new file names will appear.
6. **Rename**: Press the "Rename" button to apply changes after confirming irreversible changes message window. This message can be disabled in Menu → **Settings → Ignore Rename Warning**.
7. **Theme Selection**: Choose your preferred theme in Menu → **Settings → Theme**.

## Installation

1. Download the latest version from the [releases section](https://github.com/username/BulkFileRenamer/releases) (or compile it from source).
2. Extract the folder and run the `BulkFileRenamer.exe`.
3. No installation required.

## Version History

### v1.0.0 (Initial Release)
- Basic file renaming functionality
- Prefix and suffix support
- Hidden file skipping
- Leading zeros option
- Added theme support (Light, Dark, System)
- Added extension toggle option
- Improved real-time preview
- Added warning confirmation dialog option
### v1.1.0 (Coming Soon)
- Enhanced drag-and-drop interface with visual feedback
- Improved button state management
- Fixed theme styling issues
- Better status notifications

## Requirements
- **Windows 10+** (Windows 7 may also work, but it's not officially tested)
- **.NET Framework 4.7.2 or later** (for the compiled .exe)

## License

This project is open-source under the MIT License.

## Contact

Feel free to reach out for any questions or contributions!  
- **Author**: Jakub Ivanič
- **Email**: tunmixx@gmail.com
- **GitHub**: [github.com/tunmixx](https://github.com/tunmixx)

## Acknowledgments

Special thanks to Claude AI by Anthropic for assistance with development challenges and helping bring this project to life. This collaboration demonstrates how AI tools can help developers create practical utilities even in areas outside their primary expertise.

---

Enjoy organizing your files with **Bulk File Renamer**!