SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+08:00";

-- DB : `arcacon_parser`

-- --------------------------------------------------------

-- 테이블 구조 `ARCA_CONTENT_TB`

CREATE TABLE `ARCA_CONTENT_TB` (
  `CONTENT_TITLE` varchar(32) NOT NULL,
  `CONTENT_ID_PK` varchar(32) NOT NULL,
  `CONTENT_URL` text NOT NULL,
  `IS_VIDEO` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

-- 테이블 구조 `CONTENT_TAGS_TB`

CREATE TABLE `CONTENT_TAGS_TB` (
  `TAG_CODE_PK` int(255) NOT NULL,
  `TAG_NAME` varchar(24) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- --------------------------------------------------------


-- 테이블 구조 `CONTENT_TITLE_TB`

CREATE TABLE `CONTENT_TITLE_TB` (
  `TITLE_PK` varchar(32) NOT NULL,
  `POST_URL` text NOT NULL,
  `UPLOAD_USER` varchar(32) NOT NULL,
  `SELL_COUNT` int(24) NOT NULL,
  `TAG_LISTS` text DEFAULT NULL,
  `UPLOAD_TIME` datetime NOT NULL,
  `UPDATE_TIME` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- 덤프된 테이블의 인덱스

--
-- 테이블의 인덱스 `ARCA_CONTENT_TB`
--
ALTER TABLE `ARCA_CONTENT_TB`
  ADD PRIMARY KEY (`CONTENT_ID_PK`);

--
-- 테이블의 인덱스 `CONTENT_TAGS_TB`
--
ALTER TABLE `CONTENT_TAGS_TB`
  ADD PRIMARY KEY (`TAG_CODE_PK`);

--
-- 테이블의 인덱스 `CONTENT_TITLE_TB`
--
ALTER TABLE `CONTENT_TITLE_TB`
  ADD PRIMARY KEY (`TITLE_PK`);

--
-- 덤프된 테이블의 AUTO_INCREMENT
--

--
-- 테이블의 AUTO_INCREMENT `CONTENT_TAGS_TB`
--
ALTER TABLE `CONTENT_TAGS_TB`
  MODIFY `TAG_CODE_PK` int(255) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
COMMIT;
