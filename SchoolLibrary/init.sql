-- Расширение для bcrypt
CREATE EXTENSION IF NOT EXISTS pgcrypto;

DROP TABLE IF EXISTS borrowings    CASCADE;
DROP TABLE IF EXISTS book_authors  CASCADE;
DROP TABLE IF EXISTS books         CASCADE;
DROP TABLE IF EXISTS authors       CASCADE;
DROP TABLE IF EXISTS students      CASCADE;
DROP TABLE IF EXISTS users         CASCADE;
DROP TABLE IF EXISTS roles         CASCADE;

-- ============================
-- Роли (Librarian / Student) — справочник
-- ============================
CREATE TABLE roles (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- ============================
-- Пользователи
-- ============================
CREATE TABLE users (
    id            SERIAL PRIMARY KEY,
    login         VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    display_name  VARCHAR(200) NOT NULL,
    role_id       INTEGER NOT NULL REFERENCES roles(id) ON DELETE RESTRICT,
    created_at    TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_users_role ON users(role_id);

-- ============================
-- Авторы
-- ============================
CREATE TABLE authors (
    id        SERIAL PRIMARY KEY,
    full_name VARCHAR(200) NOT NULL,
    country   VARCHAR(100),
    bio       TEXT
);

-- ============================
-- Книги
-- ============================
CREATE TABLE books (
    id               SERIAL PRIMARY KEY,
    title            VARCHAR(300) NOT NULL,
    genre            VARCHAR(100),
    year             INTEGER,
    isbn             VARCHAR(20) UNIQUE,
    total_copies     INTEGER NOT NULL DEFAULT 1 CHECK (total_copies >= 0),
    available_copies INTEGER NOT NULL DEFAULT 1 CHECK (available_copies >= 0),
    cover_url        VARCHAR(500)
);

-- ============================
-- M:N книги ↔ авторы
-- ============================
CREATE TABLE book_authors (
    book_id   INTEGER NOT NULL REFERENCES books(id)   ON DELETE CASCADE,
    author_id INTEGER NOT NULL REFERENCES authors(id) ON DELETE CASCADE,
    PRIMARY KEY (book_id, author_id)
);

-- ============================
-- Ученики (привязка к аккаунту — опциональная)
-- ============================
CREATE TABLE students (
    id         SERIAL PRIMARY KEY,
    user_id    INTEGER UNIQUE REFERENCES users(id) ON DELETE SET NULL,
    full_name  VARCHAR(200) NOT NULL,
    class_name VARCHAR(20),
    email      VARCHAR(200) UNIQUE
);

-- ============================
-- Выдачи
-- ============================
CREATE TABLE borrowings (
    id          SERIAL PRIMARY KEY,
    book_id     INTEGER NOT NULL REFERENCES books(id)    ON DELETE CASCADE,
    student_id  INTEGER NOT NULL REFERENCES students(id) ON DELETE CASCADE,
    borrow_date DATE NOT NULL DEFAULT CURRENT_DATE,
    due_date    DATE NOT NULL,
    return_date DATE,
    CONSTRAINT chk_due_after_borrow CHECK (due_date >= borrow_date)
);

-- ============================
-- Данные
-- ============================
INSERT INTO roles (name) VALUES ('Librarian'), ('Student');

INSERT INTO users (login, password_hash, display_name, role_id) VALUES
('librarian', crypt('lib123',  gen_salt('bf', 10)), 'Мария Петровна', (SELECT id FROM roles WHERE name='Librarian')),
('student',   crypt('stud123', gen_salt('bf', 10)), 'Иванов Иван',    (SELECT id FROM roles WHERE name='Student'));

INSERT INTO authors (full_name, country, bio) VALUES
('Лев Толстой', 'Россия', 'Русский писатель, классик мировой литературы.'),
('Фёдор Достоевский', 'Россия', 'Русский писатель и мыслитель.'),
('Джоан Роулинг', 'Великобритания', 'Автор серии книг о Гарри Поттере.'),
('Антуан де Сент-Экзюпери', 'Франция', 'Писатель и лётчик.');

INSERT INTO books (title, genre, year, isbn, total_copies, available_copies) VALUES
('Война и мир', 'Роман', 1869, '978-5-04-116000-1', 5, 3),
('Преступление и наказание', 'Роман', 1866, '978-5-04-116001-8', 4, 1),
('Гарри Поттер и философский камень', 'Фэнтези', 1997, '978-5-389-07435-4', 8, 5),
('Маленький принц', 'Сказка', 1943, '978-5-699-66295-1', 6, 6),
('Анна Каренина', 'Роман', 1877, '978-5-04-116002-5', 3, 0);

INSERT INTO book_authors (book_id, author_id) VALUES
(1,1),(2,2),(3,3),(4,4),(5,1);

INSERT INTO students (user_id, full_name, class_name, email) VALUES
((SELECT id FROM users WHERE login='student'), 'Иванов Иван', '10А', 'ivanov@school.ru'),
(NULL,                                          'Петрова Анна', '9Б', 'petrova@school.ru');

INSERT INTO borrowings (book_id, student_id, borrow_date, due_date) VALUES
(2, 1, CURRENT_DATE - INTERVAL '10 days', CURRENT_DATE - INTERVAL '10 days' + INTERVAL '1 month'),
(3, 1, CURRENT_DATE - INTERVAL '35 days', CURRENT_DATE - INTERVAL '35 days' + INTERVAL '1 month');