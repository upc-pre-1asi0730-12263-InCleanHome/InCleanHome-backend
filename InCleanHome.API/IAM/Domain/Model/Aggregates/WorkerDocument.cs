using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.CreatedUpdatedDate.Contracts;

namespace InCleanHome.API.IAM.Domain.Model.Aggregates;

// ====================================================================================
// MODULE: Identity & Access Management (IAM)
// LAYER: Domain (Model / Aggregates)
//
// Represents a Worker's uploaded document entity within the IAM boundary. It acts
// as a core domain model that encapsulates the metadata and raw binary contents 
// (currently encoded as Base64) required for onboarding background checks and 
// experience verification.
//
// - Implements IEntityWithCreatedUpdatedDate to automatically track audit timestamps.
// - Designed with encapsulation in mind (private setters) to ensure state mutation
//   only occurs through the domain constructor or explicit domain behaviors.
// - Storage Strategy: Temporarily holds file contents in a string property (Base64). 
//   This keeps the interface pluggable for future refactoring to external cloud 
//   storage services (e.g., AWS S3 or Azure Blob Storage) without breaking domain contracts.
// ====================================================================================

/// <summary>
///     A document uploaded by a worker during onboarding (background check / experience).
///     The file is stored as base64 — pluggable later to S3/Blob if needed.
/// </summary>
public class WorkerDocument : IEntityWithCreatedUpdatedDate
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public string FileBase64 { get; private set; } = string.Empty;

    [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }

    public WorkerDocument() { }

    public WorkerDocument(int userId, string documentType, string fileName, string fileBase64)
    {
        UserId       = userId;
        DocumentType = documentType;
        FileName     = fileName;
        FileBase64   = fileBase64;
    }
}
