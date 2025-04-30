package com.filewatcherapp.models;

import java.time.LocalDateTime;

/**
 * Represents a file system event, including file name, extension, full path, event type, and timestamp.
 */
public class FileEvent {
    /** File name without directory path. */
    private String fileName;

    /** File extension, e.g. ".txt". */
    private String extension;

    /** Full path to the file where the event occurred. */
    private String filePath;

    /** Type of file system event: CREATED, DELETED, MODIFIED, RENAMED. */
    private String eventType;

    /** Date and time when the event occurred. */
    private LocalDateTime timestamp;

    /**
     * Constructs a new FileEvent instance.
     *
     * @param fileName  Name of the file.
     * @param extension File extension.
     * @param filePath  Full file path.
     * @param eventType Type of event.
     * @param timestamp Timestamp of the event.
     */
    public FileEvent(String fileName, String extension, String filePath, String eventType, LocalDateTime timestamp) {
        this.fileName = fileName;
        this.extension = extension;
        this.filePath = filePath;
        this.eventType = eventType;
        this.timestamp = timestamp;
    }

    // Getters and Setters

    public String getFileName() {
        return fileName;
    }

    public void setFileName(String fileName) {
        this.fileName = fileName;
    }

    public String getExtension() {
        return extension;
    }

    public void setExtension(String extension) {
        this.extension = extension;
    }

    public String getFilePath() {
        return filePath;
    }

    public void setFilePath(String filePath) {
        this.filePath = filePath;
    }

    public String getEventType() {
        return eventType;
    }

    public void setEventType(String eventType) {
        this.eventType = eventType;
    }

    public LocalDateTime getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(LocalDateTime timestamp) {
        this.timestamp = timestamp;
    }

    @Override
    public String toString() {
        return "FileEvent{" +
                "fileName='" + fileName + '\'' +
                ", extension='" + extension + '\'' +
                ", filePath='" + filePath + '\'' +
                ", eventType='" + eventType + '\'' +
                ", timestamp=" + timestamp +
                '}';
    }
}

