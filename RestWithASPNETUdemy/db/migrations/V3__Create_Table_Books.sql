CREATE TABLE IF NOT EXISTS `books` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `author` varchar(255) NOT NULL,
  `launch_date` datetime NOT NULL,
  `price` decimal(10, 2) NOT NULL,
  `title` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) 