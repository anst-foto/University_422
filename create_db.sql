-- CREATE DATABASE university_db;
-- <LOG>
CREATE SCHEMA log;
set search_path = "log";

CREATE TYPE type_event AS ENUM ('СОЗДАНИЕ', 'УДАЛЕНИЕ');
CREATE TABLE table_ddl_logs
(
    id           SERIAL     NOT NULL PRIMARY KEY,
    date_action  DATE       NOT NULL DEFAULT current_date,
    time_action  TIME       NOT NULL DEFAULT current_time,
    client_ip    inet       NULL,
    user_name    TEXT       NOT NULL,
    event_type   type_event NULL,
    object_type  TEXT       NULL,
    schema_name  TEXT       NULL,
    object_name  TEXT       NOT NULL,
    command      TEXT       NULL,
    command_tag  TEXT       NULL,
    command_text TEXT       NULL
);

CREATE OR REPLACE FUNCTION function_ddl_logger()
    RETURNS event_trigger
    LANGUAGE plpgsql
AS
$$
BEGIN
    IF tg_event = 'sql_drop' THEN
        INSERT INTO log.table_ddl_logs(client_ip, user_name, event_type, object_type, schema_name, object_name, command,
                                       command_tag, command_text)
SELECT inet_client_addr(),
       current_user,
       'УДАЛЕНИЕ',
       object_type,
       schema_name,
       object_identity,
       tg_tag,
       NULL,
       current_query()
FROM pg_event_trigger_dropped_objects()
WHERE schema_name NOT IN ('pg_temp', 'pg_toast');
ELSE
        INSERT INTO log.table_ddl_logs(client_ip, user_name, event_type, object_type, schema_name, object_name, command,
                                       command_tag, command_text)
SELECT inet_client_addr(),
       current_user,
       'СОЗДАНИЕ',
       object_type,
       schema_name,
       object_identity,
       tg_tag,
       command_tag,
       current_query()
FROM pg_event_trigger_ddl_commands()
WHERE schema_name NOT IN ('pg_temp', 'pg_toast');
END IF;
END;
$$;

CREATE EVENT TRIGGER trigger_ddl_log_create
    ON ddl_command_end
EXECUTE FUNCTION log.function_ddl_logger();

CREATE EVENT TRIGGER trigger_ddl_log_drop
    ON sql_drop
EXECUTE FUNCTION log.function_ddl_logger();

CREATE TYPE type_operation AS ENUM ('INSERT', 'UPDATE', 'DELETE', 'TRUNCATE');
CREATE TABLE table_dml_logs
(
    id             BIGSERIAL      NOT NULL PRIMARY KEY,
    schema_name    TEXT           NOT NULL,
    table_name     TEXT           NOT NULL,
    operation_type type_operation NOT NULL,
    date_operation DATE           NOT NULL DEFAULT current_date,
    time_operation TIME           NOT NULL DEFAULT current_time,
    user_name      TEXT           NOT NULL,
    old_data       jsonb          NULL,
    new_data       jsonb          NULL
);

CREATE OR REPLACE FUNCTION function_dml_logger()
    RETURNS trigger
    LANGUAGE plpgsql
AS
$$
BEGIN
    IF (tg_op = 'INSERT') THEN
        INSERT INTO log.table_dml_logs(schema_name, table_name, operation_type, user_name, old_data, new_data)
        VALUES (tg_table_schema, tg_table_name, 'INSERT', current_user, NULL, to_jsonb(NEW));
    ELSIF (tg_op = 'UPDATE') THEN
        INSERT INTO log.table_dml_logs(schema_name, table_name, operation_type, user_name, old_data, new_data)
        VALUES (tg_table_schema, tg_table_name, 'UPDATE', current_user, to_jsonb(OLD), to_jsonb(NEW));
    ELSIF (tg_op = 'DELETE') THEN
        INSERT INTO log.table_dml_logs(schema_name, table_name, operation_type, user_name, old_data, new_data)
        VALUES (tg_table_schema, tg_table_name, 'DELETE', current_user, to_jsonb(OLD), NULL);
    ELSIF (tg_op = 'TRUNCATE') THEN
        INSERT INTO log.table_dml_logs(schema_name, table_name, operation_type, user_name, old_data, new_data)
        VALUES (tg_table_schema, tg_table_name, 'TRUNCATE', current_user, NULL, NULL);
END IF;

RETURN NULL;
END;
$$;
-- </LOG>

CREATE SCHEMA test;
set search_path = "test";

-- <TABLES>

CREATE TABLE table_first_names
(
    id   SERIAL NOT NULL PRIMARY KEY,
    name TEXT   NOT NULL UNIQUE --FIXME?
);

CREATE TABLE table_last_names
(
    id   SERIAL NOT NULL PRIMARY KEY,
    name TEXT   NOT NULL UNIQUE --FIXME?
);

CREATE TABLE table_patronymics
(
    id   SERIAL NOT NULL PRIMARY KEY,
    name TEXT   NOT NULL UNIQUE --FIXME?
);

CREATE TABLE table_persons
(
    id            SERIAL NOT NULL PRIMARY KEY,
    first_name_id INT    NOT NULL,
    last_name_id  INT    NOT NULL,
    patronymic_id INT    NOT NULL,
    date_of_birth DATE   NOT NULL,
    FOREIGN KEY (first_name_id) REFERENCES table_first_names (id),
    FOREIGN KEY (last_name_id) REFERENCES table_last_names (id),
    FOREIGN KEY (patronymic_id) REFERENCES table_patronymics (id)
);

CREATE TABLE table_faculties
(
    id        SERIAL  NOT NULL PRIMARY KEY,
    title     TEXT    NOT NULL UNIQUE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE table_groups
(
    id           SERIAL  NOT NULL PRIMARY KEY,
    title        TEXT    NOT NULL,
    date_opening DATE    NOT NULL,
    date_closing DATE    NULL,
    course       INT     NOT NULL,
    faculty_id   INT     NOT NULL,
    is_active    BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (faculty_id) REFERENCES table_faculties (id)
);

CREATE TYPE type_status AS ENUM ('ЗАЧИСЛЕН', 'АКАДЕМИЧЕСКИЙ ОТПУСК', 'ОТЧИСЛЕН');
CREATE TABLE table_students
(
    id              SERIAL      NOT NULL PRIMARY KEY,
    person_id       INT         NOT NULL,
    group_id        INT         NOT NULL,
    enrollment_date DATE        NOT NULL,
    degree_date     DATE        NULL,
    status          type_status NOT NULL,
    FOREIGN KEY (person_id) REFERENCES table_persons (id),
    FOREIGN KEY (group_id) REFERENCES table_groups (id)
);

CREATE TABLE table_departments
(
    id         SERIAL  NOT NULL PRIMARY KEY,
    title      TEXT    NOT NULL,
    faculty_id INT     NOT NULL,
    is_active  BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (faculty_id) REFERENCES table_faculties (id)
);

CREATE TABLE table_subjects
(
    id          SERIAL  NOT NULL PRIMARY KEY,
    title       TEXT    NOT NULL UNIQUE,
    description TEXT    NULL,
    is_active   BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE table_departments_subjects
(
    id            SERIAL NOT NULL PRIMARY KEY,
    department_id INT    NOT NULL,
    subject_id    INT    NOT NULL,
    FOREIGN KEY (department_id) REFERENCES table_departments (id),
    FOREIGN KEY (subject_id) REFERENCES table_subjects (id)
);

-- </TABLES>

-- <VIEWS>

CREATE VIEW view_persons AS
SELECT table_persons.id            AS id,
       table_last_names.name       AS last_name,
       table_first_names.name      AS first_name,
       table_patronymics.name      AS patronymic,
       table_persons.date_of_birth AS date_of_birth
FROM table_persons
         JOIN table_first_names
              ON table_persons.first_name_id = table_first_names.id
         JOIN table_last_names
              ON table_persons.last_name_id = table_last_names.id
         JOIN table_patronymics
              ON table_persons.patronymic_id = table_patronymics.id;

CREATE VIEW view_groups AS
SELECT table_groups.id           AS id,
       table_groups.title        AS title,
       table_groups.date_opening AS date_opening,
       table_groups.date_closing AS date_closing,
       table_faculties.title     AS faculty,
       table_groups.course       AS course,
       table_groups.is_active    AS is_active
FROM table_groups
         JOIN table_faculties
              ON table_groups.faculty_id = table_faculties.id;

CREATE VIEW view_students AS
SELECT table_students.id          AS id,
       view_persons.last_name     AS last_name,
       view_persons.first_name    AS first_name,
       view_persons.patronymic    AS patronymic,
       view_persons.date_of_birth AS date_of_birth,
       view_groups.title          AS group_name,
       view_groups.course         AS course,
       view_groups.faculty        AS faculty_name,
       table_students.status      AS status
FROM table_students
         JOIN view_persons
              ON table_students.person_id = view_persons.id
         JOIN view_groups
              ON table_students.group_id = view_groups.id;

CREATE VIEW view_departments AS
SELECT table_departments.id        AS id,
       table_departments.title     AS title,
       table_faculties.title       AS faculty,
       table_departments.is_active AS is_active
FROM table_departments
         JOIN table_faculties
              ON table_departments.faculty_id = table_faculties.id;

CREATE VIEW view_departments_subjects AS
SELECT table_departments_subjects.id AS id,
       table_departments.title       AS depaertment,
       table_subjects.title          AS subject
FROM table_departments_subjects
         JOIN table_departments
              ON table_departments_subjects.department_id = table_departments.id
         JOIN table_subjects
              ON table_departments_subjects.subject_id = table_subjects.id;

-- </VIEWS>

-- <TRIGGERS>
-- set search_path = "test";

CREATE TRIGGER trigger_dml_logger_for_table_first_names
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_first_names
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_first_names
    AFTER TRUNCATE
    ON table_first_names
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_last_names
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_last_names
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_last_names
    AFTER TRUNCATE
    ON table_last_names
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_patronymics
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_patronymics
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_dml_truncate_logger_for_table_patronymics
    AFTER TRUNCATE
    ON table_patronymics
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_persons
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_persons
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_persons
    AFTER TRUNCATE
    ON table_persons
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_faculties
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_faculties
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_faculties
    AFTER TRUNCATE
    ON table_faculties
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_groups
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_groups
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_groups
    AFTER TRUNCATE
    ON table_groups
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_students
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_students
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_students
    AFTER TRUNCATE
    ON table_students
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_departments
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_departments
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_departments
    AFTER TRUNCATE
    ON table_departments
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_subjects
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_subjects
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_subjects
    AFTER TRUNCATE
    ON table_subjects
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();

CREATE TRIGGER trigger_dml_logger_for_table_departments_subjects
    AFTER INSERT OR UPDATE OR DELETE
                    ON table_departments_subjects
                        FOR EACH ROW
                        EXECUTE FUNCTION log.function_dml_logger();
CREATE TRIGGER trigger_truncate_logger_for_table_departments_subjects
    AFTER TRUNCATE
    ON table_departments_subjects
    FOR EACH STATEMENT
EXECUTE FUNCTION log.function_dml_logger();
-- </TRIGGERS>

-- <TEST DATA>
INSERT INTO test.table_first_names (id, name)
VALUES (DEFAULT, 'Андрей');

INSERT INTO test.table_last_names (id, name)
VALUES (DEFAULT, 'Старинин');

INSERT INTO test.table_patronymics (id, name)
VALUES (DEFAULT, 'Николаевич');

INSERT INTO test.table_persons (id, first_name_id, last_name_id, patronymic_id, date_of_birth)
VALUES (DEFAULT, 1, 1, 1, '1986-02-18');

INSERT INTO test.table_faculties (id, title, is_active)
VALUES (DEFAULT, 'РПО', DEFAULT);

INSERT INTO test.table_groups (id, title, date_opening, date_closing, course, faculty_id, is_active)
VALUES (DEFAULT, 'ПВ421', '2025-01-01', null, 1, 1, DEFAULT);

INSERT INTO test.table_students (id, person_id, group_id, enrollment_date, degree_date, status)
VALUES (DEFAULT, 1, 1, '2025-01-01', null, 'ЗАЧИСЛЕН'::test.type_status);

UPDATE test.table_students
SET status = 'ОТЧИСЛЕН'::test.type_status
WHERE id = 1;
-- </TEST DATA>