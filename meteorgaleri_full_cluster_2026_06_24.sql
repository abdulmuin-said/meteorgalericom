--
-- PostgreSQL database cluster dump
--

\restrict VDQycae2n7TJoKTqO6xuRrBRZxFAiFpr0sCEwQ62SpbXbJJe5l1bN3F4nG1wmeI

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE kanvasuser;
ALTER ROLE kanvasuser WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS PASSWORD 'SCRAM-SHA-256$4096:UiyZu0VCa5MgONkwl2DyBA==$V6CoDInpCAk4p5WCw5w6Y7XPOOwbXRa6fNFzrdDS5cw=:zg1PkCxV3Ep7fmSq6t1Kz3kLAoD3RwA8zQNdk0WY3xE=';

--
-- User Configurations
--








\unrestrict VDQycae2n7TJoKTqO6xuRrBRZxFAiFpr0sCEwQ62SpbXbJJe5l1bN3F4nG1wmeI

--
-- Databases
--

--
-- Database "template1" dump
--

\connect template1

--
-- PostgreSQL database dump
--

\restrict WKfi3fH7et2CLS0Nqs4foRHSSj5OtAudbFMzTqIppL5a4KJy5zWL9envucBxBm9

-- Dumped from database version 16.14
-- Dumped by pg_dump version 16.14

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- PostgreSQL database dump complete
--

\unrestrict WKfi3fH7et2CLS0Nqs4foRHSSj5OtAudbFMzTqIppL5a4KJy5zWL9envucBxBm9

--
-- Database "kanvasdb" dump
--

--
-- PostgreSQL database dump
--

\restrict vsPWm3evuqrC4NQQDA6YFuM2fCLP3ovlk5kPPOWPxnUHQoCB2EJbziZEHzD3Ucq

-- Dumped from database version 16.14
-- Dumped by pg_dump version 16.14

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: kanvasdb; Type: DATABASE; Schema: -; Owner: kanvasuser
--

CREATE DATABASE kanvasdb WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE kanvasdb OWNER TO kanvasuser;

\unrestrict vsPWm3evuqrC4NQQDA6YFuM2fCLP3ovlk5kPPOWPxnUHQoCB2EJbziZEHzD3Ucq
\connect kanvasdb
\restrict vsPWm3evuqrC4NQQDA6YFuM2fCLP3ovlk5kPPOWPxnUHQoCB2EJbziZEHzD3Ucq

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- PostgreSQL database dump complete
--

\unrestrict vsPWm3evuqrC4NQQDA6YFuM2fCLP3ovlk5kPPOWPxnUHQoCB2EJbziZEHzD3Ucq

--
-- Database "postgres" dump
--

\connect postgres

--
-- PostgreSQL database dump
--

\restrict 0cO9yXvSri11Bhl4pouqoO7hb8gu9Re67I91EthPm1VczJYbWj8viH6PciukoGb

-- Dumped from database version 16.14
-- Dumped by pg_dump version 16.14

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- PostgreSQL database dump complete
--

\unrestrict 0cO9yXvSri11Bhl4pouqoO7hb8gu9Re67I91EthPm1VczJYbWj8viH6PciukoGb

--
-- PostgreSQL database cluster dump complete
--

