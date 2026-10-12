# UNITE repository instructions

Before changing this repository, read the shared UNITE instructions and
relevant project documentation maintained in
[dkfz-unite/.github](https://github.com/dkfz-unite/.github/blob/main/README.md).

1. Prefer the local shared checkout: `../unite-meta/AGENTS.md`, relative to this
   repository root. A sibling `.github/AGENTS.md` or a user-provided path is also
   valid. Resolve its document links relative to that shared checkout.
2. Read the shared `AGENTS.md`, project overview, repository map, and topic
   documents relevant to the task, following the shared README's task index.
3. If no shared checkout is accessible, use the GitHub versions beginning with
   [shared AGENTS.md](https://github.com/dkfz-unite/.github/blob/main/AGENTS.md).
   If those are unavailable too, report the missing context and proceed only
   with what local documentation/code establishes; do not invent shared rules.
4. Read this repository's README and relevant local docs; inspect Git status.
   More specific instructions here supplement the shared guidance.

Keep common rules in `.github`; keep component details in this repository.

## Images feed

Status: reviewed with the owner (2026-10-12).

The images feed ingests imaging metadata (currently MR images) and radiomics
features, writes them to the domain database and builds the images index.
It follows the same pattern as the
[omics feed](../unite-feed-omics/AGENTS.md) (submission staging, review, SQL
writers, indexing workers) with far less code: no annotation and no resources.
Background: [Data ingestion](../unite-meta/docs/data-ingestion.md),
[Processing pipeline](../unite-meta/docs/processing-pipeline.md),
[Domain model](../unite-meta/docs/domain.md#images-and-biological-specimens).

### Projects

| Project | Contents |
| --- | --- |
| `Unite.Images.Feed.Web` | Controllers, submission models (binders, converters, validators), Mongo staging (`Submissions/`), workers and submission handlers |
| `Unite.Images.Feed` | SQL writers (`ImagesWriter`, `AnalysisWriter`), `ImagesRemover`, repositories (images, radiomics features) |
| `Unite.Images.Indices` | Image index creator and mapping |

There are no test projects.

### Build and run

```sh
dotnet build Unite.Images.Feed.sln
dotnet run --project Unite.Images.Feed.Web --launch-profile Unite.Radiology.Feed.Web
```

- Local URL `http://localhost:5102`; Docker `images.feed.unite.net`. Needs
  PostgreSQL, MongoDB and Elasticsearch. The launch profile still carries the
  old name `Unite.Radiology.Feed.Web`.
- To test against the running platform, stop `unite.feed.images` first; see
  [Running a service locally](../unite-meta/docs/development-validation.md#running-a-service-locally-instead-of-its-container).

### Configuration and API

Environment variables are in the [README](README.md); endpoints and models in
[Docs/api.md](Docs/api.md). Routes, task types and handlers:

| Route | Task type | Notes |
| --- | --- | --- |
| `api/entries/mr[/tsv]` | `MR` | MR image metadata; creates missing donors |
| `api/analysis/radiomics[/tsv]` | `IMG_RAD` | Radiomics features per image sample; replaced per sample on re-upload |
| `api/entry/{id}` (DELETE) | – | Deletes an image with its data and index entry |
| `api/indexing` (POST) | – | Administrator reindexing: deletes the index and recreates tasks |

`review=false` creates the submission task as `Prepared`. CT images are a
planned future type (owner, 2026-10-12): `ImageType.CT` and the index flag
`Cts` exist, but there is no CT endpoint, model or portal page yet. Add CT
alongside MR (controller, model, handler, writer) when it is implemented.

### Processing

`SubmissionsWorker` runs the MR and radiomics submission handlers;
`IndexingWorker` builds image documents. Writers create missing donors in the
default project `Other` (public), as in the other feeds.

### Shared libraries

`Unite.Data.Context` 12.10.33, `Unite.Indices.Context` 13.10.0 (latest
13.10.20), `Unite.Cache` 1.0.0 (latest 1.3.0), `Unite.Essentials.Tsv` 4.2.0.

### CI

`docker-images.yml` builds amd64 and arm64 images under one manifest on pushes
to `main`.

### Known issues

Defects are tracked in the private Jira; see the shared
[Track defects](../unite-meta/AGENTS.md#track-defects).
