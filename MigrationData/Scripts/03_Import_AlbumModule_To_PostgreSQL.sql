\echo ========================================
\echo Album Module Import Started
\echo Target: PostgreSQL musicstoremodern
\echo ========================================

\echo Cleaning target tables...

TRUNCATE TABLE public."Albums", public."Artists", public."Genres"
RESTART IDENTITY CASCADE;

\echo Importing Genres...

\copy public."Genres" ("GenreId", "Name", "Description") FROM 'G:/Projects/MigrationData/genres.csv' WITH (FORMAT csv, HEADER true, ENCODING 'UTF8');

\echo Importing Artists...

\copy public."Artists" ("ArtistId", "Name") FROM 'G:/Projects/MigrationData/artists.csv' WITH (FORMAT csv, HEADER true, ENCODING 'UTF8');

\echo Importing Albums...

\copy public."Albums" ("AlbumId", "GenreId", "ArtistId", "Title", "Price", "AlbumArtUrl") FROM 'G:/Projects/MigrationData/albums.csv' WITH (FORMAT csv, HEADER true, ENCODING 'UTF8');

\echo Resetting identity sequences...

SELECT setval(
    pg_get_serial_sequence('public."Genres"', 'GenreId'),
    COALESCE((SELECT MAX("GenreId") FROM public."Genres"), 1),
    true
);

SELECT setval(
    pg_get_serial_sequence('public."Artists"', 'ArtistId'),
    COALESCE((SELECT MAX("ArtistId") FROM public."Artists"), 1),
    true
);

SELECT setval(
    pg_get_serial_sequence('public."Albums"', 'AlbumId'),
    COALESCE((SELECT MAX("AlbumId") FROM public."Albums"), 1),
    true
);

\echo Validating row counts...

SELECT 'Genres' AS table_name, COUNT(*) AS total_rows FROM public."Genres"
UNION ALL
SELECT 'Artists', COUNT(*) FROM public."Artists"
UNION ALL
SELECT 'Albums', COUNT(*) FROM public."Albums"
ORDER BY table_name;

\echo Validating sample joined Album data...

SELECT 
    a."AlbumId",
    a."Title",
    ar."Name" AS "Artist",
    g."Name" AS "Genre",
    a."Price"
FROM public."Albums" a
JOIN public."Artists" ar
    ON a."ArtistId" = ar."ArtistId"
JOIN public."Genres" g
    ON a."GenreId" = g."GenreId"
ORDER BY a."AlbumId"
LIMIT 10;

\echo ========================================
\echo Album Module Import Finished
\echo ========================================