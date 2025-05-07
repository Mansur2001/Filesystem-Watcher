package com.filewatcherapp.utils;

import java.io.IOException;
import java.nio.file.DirectoryStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

/**
 * Utility class for quickly finding files in a directory tree by extension or pattern.
 */
public class FileFinder {
    /**
     * Recursively searches for files under the given directory matching the specified extension.
     *
     * @param rootDir   the starting directory path as a string
     * @param extension the file extension to match (e.g. "txt" or ".txt")
     * @return a list of absolute file paths as strings
     * @throws IOException if an I/O error occurs
     */
    public static List<String> findFilesByExtension(String rootDir, String extension) throws IOException {
        // Normalize extension: ensure it starts with a dot
        String ext = extension.startsWith(".") ? extension : "." + extension;
        List<String> results = new ArrayList<>();
        Path startPath = Paths.get(rootDir);
        // Traverse the directory tree
        try (DirectoryStream<Path> stream = Files.newDirectoryStream(startPath)) {
            for (Path entry : stream) {
                if (Files.isDirectory(entry)) {
                    // Recurse into subdirectory
                    results.addAll(findFilesByExtension(entry.toString(), extension));
                } else if (entry.getFileName().toString().endsWith(ext)) {
                    results.add(entry.toAbsolutePath().toString());
                }
            }
        }
        return results;
    }

    /**
     * Main method for quick testing.
     */
    public static void main(String[] args) {
        String dir = "."; // current directory
        String ext = "java";
        try {
            List<String> files = findFilesByExtension(dir, ext);
            files.forEach(System.out::println);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}

