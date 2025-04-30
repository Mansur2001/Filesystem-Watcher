package com.filewatcherapp.models;

import java.time.LocalDateTime;

/**
 * Encapsulates the criteria for querying file events from the database.
 */
public class QueryCriteria {
    /** Optional start of date range (inclusive). */
    private LocalDateTime startDate;

    /** Optional end of date range (inclusive). */
    private LocalDateTime endDate;

    /** Optional file extension filter (e.g. ".txt" or "txt"). */
    private String extension;

    /** Optional event type filter: CREATED, DELETED, MODIFIED, RENAMED. */
    private String eventType;

    /** Optional directory or path filter. */
    private String directoryPath;

    public QueryCriteria() {
        // Default constructor
    }

    public LocalDateTime getStartDate() {
        return startDate;
    }

    public void setStartDate(LocalDateTime startDate) {
        this.startDate = startDate;
    }

    public LocalDateTime getEndDate() {
        return endDate;
    }

    public void setEndDate(LocalDateTime endDate) {
        this.endDate = endDate;
    }

    public String getExtension() {
        return extension;
    }

    public void setExtension(String extension) {
        this.extension = extension;
    }

    public String getEventType() {
        return eventType;
    }

    public void setEventType(String eventType) {
        this.eventType = eventType;
    }

    public String getDirectoryPath() {
        return directoryPath;
    }

    public void setDirectoryPath(String directoryPath) {
        this.directoryPath = directoryPath;
    }

    @Override
    public String toString() {
        return "QueryCriteria{" +
                "startDate=" + startDate +
                ", endDate=" + endDate +
                ", extension='" + extension + '\'' +
                ", eventType='" + eventType + '\'' +
                ", directoryPath='" + directoryPath + '\'' +
                '}';
    }
}
