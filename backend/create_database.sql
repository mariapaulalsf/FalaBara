CREATE DATABASE falabara_db;

\c falabara_db;

CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(200) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "Type" INTEGER NOT NULL DEFAULT 0,
    "Cpf" VARCHAR(14),
    "Department" VARCHAR(200),
    "EmployeeName" VARCHAR(200)
);

CREATE TABLE "Complaints" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Description" TEXT NOT NULL,
    "Location" VARCHAR(200) NOT NULL,
    "Neighborhood" VARCHAR(200) NOT NULL,
    "PhotoUrl" TEXT,
    "Category" INTEGER NOT NULL,
    "Severity" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ResolvedAt" TIMESTAMP,
    "UpVotes" INTEGER NOT NULL DEFAULT 0,
    "DownVotes" INTEGER NOT NULL DEFAULT 0,
    "UserId" INTEGER NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id")
);

CREATE TABLE "Comments" (
    "Id" SERIAL PRIMARY KEY,
    "Content" TEXT NOT NULL,
    "PhotoUrl" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UserId" INTEGER NOT NULL,
    "ComplaintId" INTEGER NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id"),
    FOREIGN KEY ("ComplaintId") REFERENCES "Complaints"("Id")
);

CREATE INDEX "IX_Complaints_UserId" ON "Complaints"("UserId");
CREATE INDEX "IX_Comments_UserId" ON "Comments"("UserId");
CREATE INDEX "IX_Comments_ComplaintId" ON "Comments"("ComplaintId");
