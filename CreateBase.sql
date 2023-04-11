
--================================================================================================================================================================
-- Отделы
--================================================================================================================================================================

CREATE TABLE [dbo].[otdels](
	[idOtdel] [int] IDENTITY(1,1) NOT NULL,
	[ot_name] [varchar](100) NULL,
	[ot_parent] [int] NULL,
	[ot_itr] [int] NOT NULL,
	[ot_sort] [int] NULL,
 CONSTRAINT [PK_otdels] PRIMARY KEY CLUSTERED 
(
	[idOtdel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[otdels] ADD  CONSTRAINT [DF_otdels_ot_itr]  DEFAULT ((0)) FOR [ot_itr]

ALTER TABLE [dbo].[otdels]  WITH CHECK ADD  CONSTRAINT [FK_otdels_otdels] FOREIGN KEY([ot_parent])
REFERENCES [dbo].[otdels] ([idOtdel])

ALTER TABLE [dbo].[otdels] CHECK CONSTRAINT [FK_otdels_otdels]

--================================================================================================================================================================
-- Персонал
--================================================================================================================================================================

CREATE TABLE [dbo].[personal](
	[idPerson] [int] IDENTITY(1,1) NOT NULL,
	[p_tab_number] [varchar](50) NULL,
	[p_lastname] [varchar](50) NULL,
	[p_name] [varchar](50) NULL,
	[p_midname] [varchar](50) NULL,
	[p_profession] [varchar](150) NULL,
	[p_otdel_id] [int] NULL,
	[p_cat_id] [int] NULL,
	[p_delete] [bit] NOT NULL,
	[p_type] [int] NOT NULL,
	[p_stavka] [numeric](5, 1) NOT NULL,
	[p_premTarif] [numeric](10, 2) NULL,
	[p_type_id] [int] NOT NULL,
	[p_notPrintModel] [bit] NULL,
 CONSTRAINT [PK_personal] PRIMARY KEY CLUSTERED 
(
	[idPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[personal] ADD  CONSTRAINT [DF_personal_p_delete]  DEFAULT ((0)) FOR [p_delete]
ALTER TABLE [dbo].[personal] ADD  CONSTRAINT [DF_personal_p_type]  DEFAULT ((0)) FOR [p_type]
ALTER TABLE [dbo].[personal] ADD  CONSTRAINT [DF_personal_p_stavka]  DEFAULT ((1)) FOR [p_stavka]
ALTER TABLE [dbo].[personal] ADD  CONSTRAINT [DF_personal_p_type_id]  DEFAULT ((0)) FOR [p_type_id]
ALTER TABLE [dbo].[personal]  WITH CHECK ADD  CONSTRAINT [FK_personal_otdels] FOREIGN KEY([p_otdel_id])
REFERENCES [dbo].[otdels] ([idOtdel])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[personal] CHECK CONSTRAINT [FK_personal_otdels]

--================================================================================================================================================================
-- Дополнитльные работы
--================================================================================================================================================================

CREATE TABLE [dbo].[AddWorks](
	[aw_Id] [int] IDENTITY(1,1) NOT NULL,
	[aw_Name] [varchar](150) NULL,
	[aw_Tarif] [numeric](18, 2) NULL,
	[aw_IsRelateHours] [bit] NULL,
 CONSTRAINT [PK_AddWorks] PRIMARY KEY CLUSTERED 
(
	[aw_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--================================================================================================================================================================
-- Пользователи
--================================================================================================================================================================

CREATE TABLE [dbo].[users](
	[idUser] [int] IDENTITY(1,1) NOT NULL,
	[u_login] [varchar](50) NULL,
	[u_pass] [varchar](50) NULL,
	[u_pass2] [varchar](50) NULL,
	[u_role] [int] NOT NULL,
	[u_fio] [varchar](150) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_u_role]  DEFAULT ((2)) FOR [u_role]

INSERT INTO [dbo].[users] ([u_login],[u_pass] ,[u_role] ,[u_fio]) VALUES ('Admin','123',0,'Администратор')

--================================================================================================================================================================
-- Набор грейдов
--================================================================================================================================================================
CREATE TABLE [dbo].[CategorySet](
	[cg_id] [int] IDENTITY(1,1) NOT NULL,
	[cg_date] [datetime] NULL,
	[cg_value] [numeric](6, 2) NULL,
 CONSTRAINT [PK_CategorySet] PRIMARY KEY CLUSTERED 
(
	[cg_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO [dbo].[CategorySet] ([cg_date] ,[cg_value]) VALUES('01.01.2022',0)

--================================================================================================================================================================
-- Грейды
--================================================================================================================================================================
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idCategory] [int] NOT NULL,
	[cat_setId] [int] NOT NULL,
	[cat_tarif] [numeric](18, 2) NULL,
	[cat_prem_tarif] [numeric](18, 2) NULL,
	[cat_min_level] [numeric](18, 2) NULL,
	[cat_max_level] [numeric](18, 2) NULL,
 CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_category] UNIQUE NONCLUSTERED 
(
	[idCategory] ASC,
	[cat_setId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[category]  WITH CHECK ADD  CONSTRAINT [FK_category_CategorySet] FOREIGN KEY([cat_setId])
REFERENCES [dbo].[CategorySet] ([cg_id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[category] CHECK CONSTRAINT [FK_category_CategorySet]


--================================================================================================================================================================
-- Календарь
--================================================================================================================================================================

CREATE TABLE [dbo].[calendar](
	[idCal] [int] IDENTITY(1,1) NOT NULL,
	[cal_year] [int] NULL,
	[cal_date] [date] NOT NULL,
	[cal_type] [int] NOT NULL,
 CONSTRAINT [PK_calendar] PRIMARY KEY CLUSTERED 
(
	[idCal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]





--================================================================================================================================================================
-- Список общих начислений
--================================================================================================================================================================
CREATE TABLE [dbo].[GeneralCharges](
	[gen_Id] [int] NOT NULL,
	[gen_Name] [varchar](150) NULL,
 CONSTRAINT [PK_GeneralCharges] PRIMARY KEY CLUSTERED 
(
	[gen_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--================================================================================================================================================================
-- Общие начисления по месяцам
--================================================================================================================================================================
CREATE TABLE [dbo].[GenChargMonth](
	[gm_Id] [int] IDENTITY(1,1) NOT NULL,
	[gm_GenId] [int] NULL,
	[gm_Month] [int] NULL,
	[gm_Year] [int] NULL,
	[gm_Value] [numeric](18, 2) NULL,
 CONSTRAINT [PK_GenChargMonth] PRIMARY KEY CLUSTERED 
(
	[gm_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[GenChargMonth]  WITH CHECK ADD  CONSTRAINT [FK_GenChargMonth_GeneralCharges] FOREIGN KEY([gm_GenId])
REFERENCES [dbo].[GeneralCharges] ([gen_Id])

ALTER TABLE [dbo].[GenChargMonth] CHECK CONSTRAINT [FK_GenChargMonth_GeneralCharges]


--================================================================================================================================================================
-- Модель
--================================================================================================================================================================

CREATE TABLE [dbo].[Mod](
	[m_Id] [int] IDENTITY(1,1) NOT NULL,
	[m_otdelId] [int] NOT NULL,
	[m_year] [int] NOT NULL,
	[m_month] [int] NOT NULL,
	[m_author] [int] NOT NULL,
	[m_HoursFromFP] [numeric](18, 2) NULL,
	[m_IsClosed] [bit] NULL,
 CONSTRAINT [PK_Mod_1] PRIMARY KEY CLUSTERED 
(
	[m_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Mod]  WITH CHECK ADD  CONSTRAINT [FK_Mod_otdels] FOREIGN KEY([m_otdelId])
REFERENCES [dbo].[otdels] ([idOtdel])
GO

ALTER TABLE [dbo].[Mod] CHECK CONSTRAINT [FK_Mod_otdels]
GO

ALTER TABLE [dbo].[Mod]  WITH CHECK ADD  CONSTRAINT [FK_Mod_users] FOREIGN KEY([m_author])
REFERENCES [dbo].[users] ([idUser])
GO

ALTER TABLE [dbo].[Mod] CHECK CONSTRAINT [FK_Mod_users]


--================================================================================================================================================================
-- Персонал модели
--================================================================================================================================================================

CREATE TABLE [dbo].[ModPerson](
	[md_Id] [int] IDENTITY(1,1) NOT NULL,
	[md_modId] [int] NULL,
	[md_personalId] [int] NOT NULL,
	[md_hoursFP] [numeric](18, 2) NULL,
	[md_premFP] [numeric](18, 2) NULL,
	[md_prem1_tarif] [numeric](18, 2) NULL,
	[md_bonus_exec] [bit] NOT NULL,
	[md_bonus_max] [numeric](18, 2) NULL,
	[md_bonus_proc] [numeric](18, 2) NULL,
	[md_prem_otdel] [numeric](18, 2) NULL,
	[md_prem_otdel_proc] [numeric](18, 2) NULL,
	[md_prem_stimul_name] [varchar](max) NULL,
	[md_tarif_offDay] [numeric](18, 2) NULL,
	[md_group] [varchar](20) NULL,
	[md_sumFromFP] [numeric](18, 2) NULL,
	[md_kvalif_prem] [numeric](18, 2) NULL,
	[md_kvalif_name] [varchar](max) NULL,
	[md_kvalif_tarif] [numeric](18, 2) NULL,
	[md_kvalif_proc] [numeric](10, 2) NULL,
	[md_cat_prem_tarif] [numeric](18, 2) NULL,
	[md_quality_check] [bit] NOT NULL,
	[md_person_achiev] [numeric](18, 4) NULL,
	[md_Oklad] [numeric](18, 4) NULL,
	[md_bolnich] [numeric](18, 4) NULL,
	[md_RealPay] [numeric](18, 4) NULL,
	[md_cat_tarif] [numeric](18, 4) NULL,
	[md_compens] [numeric](18, 4) NULL,
 CONSTRAINT [PK_Mod] PRIMARY KEY CLUSTERED 
(
	[md_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[ModPerson] ADD  CONSTRAINT [DF_ModPerson_md_bonus_exec]  DEFAULT ((0)) FOR [md_bonus_exec]

ALTER TABLE [dbo].[ModPerson] ADD  CONSTRAINT [DF_ModPerson_md_quality_check]  DEFAULT ((0)) FOR [md_quality_check]

ALTER TABLE [dbo].[ModPerson]  WITH CHECK ADD  CONSTRAINT [FK_ModPerson_Mod] FOREIGN KEY([md_modId])
REFERENCES [dbo].[Mod] ([m_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[ModPerson] CHECK CONSTRAINT [FK_ModPerson_Mod]

ALTER TABLE [dbo].[ModPerson]  WITH CHECK ADD  CONSTRAINT [FK_ModPerson_personal] FOREIGN KEY([md_personalId])
REFERENCES [dbo].[personal] ([idPerson])

ALTER TABLE [dbo].[ModPerson] CHECK CONSTRAINT [FK_ModPerson_personal]



--================================================================================================================================================================
-- Доп работы для персонала модели
--================================================================================================================================================================
CREATE TABLE [dbo].[ModPersonAddWorks](
	[mp_Id] [int] NOT NULL,
	[aw_id] [int] NOT NULL
) ON [PRIMARY]

ALTER TABLE [dbo].[ModPersonAddWorks]  WITH CHECK ADD  CONSTRAINT [FK_ModPersonAddWorks_AddWorks] FOREIGN KEY([aw_id])
REFERENCES [dbo].[AddWorks] ([aw_Id])

ALTER TABLE [dbo].[ModPersonAddWorks] CHECK CONSTRAINT [FK_ModPersonAddWorks_AddWorks]

ALTER TABLE [dbo].[ModPersonAddWorks]  WITH CHECK ADD  CONSTRAINT [FK_ModPersonAddWorks_ModPerson] FOREIGN KEY([mp_Id])
REFERENCES [dbo].[ModPerson] ([md_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[ModPersonAddWorks] CHECK CONSTRAINT [FK_ModPersonAddWorks_ModPerson]


--================================================================================================================================================================
-- Прикрепленные файлы
--================================================================================================================================================================

CREATE TABLE [dbo].[AttachFile](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[mod_id] [int] NULL,
	[FileName] [varchar](max) NULL,
 CONSTRAINT [PK_AttachFile] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AttachFile]  WITH CHECK ADD  CONSTRAINT [FK_AttachFile_Mod] FOREIGN KEY([mod_id])
REFERENCES [dbo].[Mod] ([m_Id])
GO

ALTER TABLE [dbo].[AttachFile] CHECK CONSTRAINT [FK_AttachFile_Mod]


--================================================================================================================================================================
-- Отпуска
--================================================================================================================================================================

CREATE TABLE [dbo].[Otpusk](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[o_number] [varchar](20) NULL,
	[o_dateCreate] [datetime] NULL,
	[o_year] [int] NULL,
	[o_otdelId] [int] NOT NULL,
	[o_author] [varchar](50) NULL,
 CONSTRAINT [PK_Otpusk] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Otpusk]  WITH CHECK ADD  CONSTRAINT [FK_Otpusk_otdels] FOREIGN KEY([o_otdelId])
REFERENCES [dbo].[otdels] ([idOtdel])

ALTER TABLE [dbo].[Otpusk] CHECK CONSTRAINT [FK_Otpusk_otdels]


--================================================================================================================================================================
-- Дни отпусков для сотрудников
--================================================================================================================================================================
CREATE TABLE [dbo].[OtpuskDays](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[od_otpuskPersonId] [int] NOT NULL,
	[od_StartDate] [datetime] NOT NULL,
	[od_EndDate] [datetime] NOT NULL,
 CONSTRAINT [PK_OtpuskDays] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


--================================================================================================================================================================
-- Дни отпусков для сотрудников
--================================================================================================================================================================
CREATE TABLE [dbo].[OtpuskPerson](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[op_otpuskId] [int] NOT NULL,
	[op_personId] [int] NOT NULL,
 CONSTRAINT [PK_OtpuskPerson] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[OtpuskPerson]  WITH CHECK ADD  CONSTRAINT [FK_OtpuskPerson_Otpusk] FOREIGN KEY([op_otpuskId])
REFERENCES [dbo].[Otpusk] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[OtpuskPerson] CHECK CONSTRAINT [FK_OtpuskPerson_Otpusk]

ALTER TABLE [dbo].[OtpuskPerson]  WITH CHECK ADD  CONSTRAINT [FK_OtpuskPerson_personal] FOREIGN KEY([op_personId])
REFERENCES [dbo].[personal] ([idPerson])

ALTER TABLE [dbo].[OtpuskPerson] CHECK CONSTRAINT [FK_OtpuskPerson_personal]


--================================================================================================================================================================
-- Отдельный отдел
--================================================================================================================================================================

CREATE TABLE [dbo].[Separate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[s_otdelId] [int] NOT NULL,
	[s_year] [int] NOT NULL,
	[s_month] [int] NOT NULL,
	[s_author] [varchar](50) NULL,
 CONSTRAINT [PK_Separate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Separate]  WITH CHECK ADD  CONSTRAINT [FK_Separate_otdels] FOREIGN KEY([s_otdelId])
REFERENCES [dbo].[otdels] ([idOtdel])

ALTER TABLE [dbo].[Separate] CHECK CONSTRAINT [FK_Separate_otdels]


--================================================================================================================================================================
-- Персонал отдельного отдела
--================================================================================================================================================================
CREATE TABLE [dbo].[SeparPerson](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sp_separId] [int] NULL,
	[sp_personalId] [int] NULL,
	[sp_oklad] [decimal](18, 2) NULL,
	[sp_premia] [decimal](18, 2) NULL,
 CONSTRAINT [PK_SeparPerson] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[SeparPerson]  WITH CHECK ADD  CONSTRAINT [FK_SeparPerson_personal] FOREIGN KEY([sp_personalId])
REFERENCES [dbo].[personal] ([idPerson])

ALTER TABLE [dbo].[SeparPerson] CHECK CONSTRAINT [FK_SeparPerson_personal]

ALTER TABLE [dbo].[SeparPerson]  WITH CHECK ADD  CONSTRAINT [FK_SeparPerson_Separate] FOREIGN KEY([sp_separId])
REFERENCES [dbo].[Separate] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[SeparPerson] CHECK CONSTRAINT [FK_SeparPerson_Separate]


--================================================================================================================================================================
-- Смены
--================================================================================================================================================================

CREATE TABLE [dbo].[smena](
	[sm_Id] [int] IDENTITY(1,1) NOT NULL,
	[sm_Number] [varchar](20) NULL,
	[sm_DateCreate] [datetime] NOT NULL,
	[sm_Month] [int] NOT NULL,
	[sm_Year] [int] NOT NULL,
	[sm_UserId] [int] NOT NULL,
	[sm_OtdelId] [int] NOT NULL,
	[sm_IsClosed] [bit] NULL,
 CONSTRAINT [PK_smena] PRIMARY KEY CLUSTERED 
(
	[sm_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[smena]  WITH CHECK ADD  CONSTRAINT [FK_smena_otdels] FOREIGN KEY([sm_OtdelId])
REFERENCES [dbo].[otdels] ([idOtdel])

ALTER TABLE [dbo].[smena] CHECK CONSTRAINT [FK_smena_otdels]

ALTER TABLE [dbo].[smena]  WITH CHECK ADD  CONSTRAINT [FK_smena_users] FOREIGN KEY([sm_UserId])
REFERENCES [dbo].[users] ([idUser])

ALTER TABLE [dbo].[smena] CHECK CONSTRAINT [FK_smena_users]



--================================================================================================================================================================
-- Персонал смен
--================================================================================================================================================================

CREATE TABLE [dbo].[SmenaPerson](
	[sp_Id] [int] IDENTITY(1,1) NOT NULL,
	[sp_PersonId] [int] NOT NULL,
	[sp_SmenaId] [int] NOT NULL,
	[sp_Group] [int] NULL,
 CONSTRAINT [PK_SmenaPerson] PRIMARY KEY CLUSTERED 
(
	[sp_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[SmenaPerson]  WITH CHECK ADD  CONSTRAINT [FK_SmenaPerson_personal] FOREIGN KEY([sp_PersonId])
REFERENCES [dbo].[personal] ([idPerson])

ALTER TABLE [dbo].[SmenaPerson] CHECK CONSTRAINT [FK_SmenaPerson_personal]

ALTER TABLE [dbo].[SmenaPerson]  WITH CHECK ADD  CONSTRAINT [FK_SmenaPerson_smena] FOREIGN KEY([sp_SmenaId])
REFERENCES [dbo].[smena] ([sm_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[SmenaPerson] CHECK CONSTRAINT [FK_SmenaPerson_smena]



--================================================================================================================================================================
-- Персонал смен дни
--================================================================================================================================================================

CREATE TABLE [dbo].[SmenaDay](
	[sd_Id] [int] IDENTITY(1,1) NOT NULL,
	[sd_SmenaPersonId] [int] NULL,
	[sd_Day] [int] NOT NULL,
	[sd_Kind] [int] NOT NULL,
 CONSTRAINT [PK_SmenaDay] PRIMARY KEY CLUSTERED 
(
	[sd_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[SmenaDay]  WITH CHECK ADD  CONSTRAINT [FK_SmenaDay_SmenaPerson] FOREIGN KEY([sd_SmenaPersonId])
REFERENCES [dbo].[SmenaPerson] ([sp_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[SmenaDay] CHECK CONSTRAINT [FK_SmenaDay_SmenaPerson]


--================================================================================================================================================================
-- Табель
--================================================================================================================================================================

CREATE TABLE [dbo].[tabel](
	[idTabel] [int] IDENTITY(1,1) NOT NULL,
	[t_otdel_id] [int] NOT NULL,
	[t_number] [varchar](50) NULL,
	[t_date_create] [datetime] NULL,
	[t_year] [int] NOT NULL,
	[t_month] [int] NOT NULL,
	[t_status] [int] NULL,
	[t_author] [varchar](180) NULL,
	[t_author_id] [int] NULL,
	[t_IsClosed] [bit] NULL,
 CONSTRAINT [PK_tabel] PRIMARY KEY CLUSTERED 
(
	[idTabel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tabel]  WITH CHECK ADD  CONSTRAINT [FK_tabel_otdels] FOREIGN KEY([t_otdel_id])
REFERENCES [dbo].[otdels] ([idOtdel])

ALTER TABLE [dbo].[tabel] CHECK CONSTRAINT [FK_tabel_otdels]

ALTER TABLE [dbo].[tabel]  WITH CHECK ADD  CONSTRAINT [FK_tabel_users] FOREIGN KEY([t_author_id])
REFERENCES [dbo].[users] ([idUser])

ALTER TABLE [dbo].[tabel] CHECK CONSTRAINT [FK_tabel_users]



--================================================================================================================================================================
-- Персонал табеля
--================================================================================================================================================================

CREATE TABLE [dbo].[tabelPerson](
	[tp_Id] [int] IDENTITY(1,1) NOT NULL,
	[tp_person_id] [int] NOT NULL,
	[tp_tabel_id] [int] NOT NULL,
	[tp_AddingHours] [numeric](5, 1) NULL,
 CONSTRAINT [PK_days] PRIMARY KEY CLUSTERED 
(
	[tp_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tabelPerson]  WITH CHECK ADD  CONSTRAINT [FK_days_personal] FOREIGN KEY([tp_person_id])
REFERENCES [dbo].[personal] ([idPerson])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[tabelPerson] CHECK CONSTRAINT [FK_days_personal]

ALTER TABLE [dbo].[tabelPerson]  WITH CHECK ADD  CONSTRAINT [FK_days_tabel] FOREIGN KEY([tp_tabel_id])
REFERENCES [dbo].[tabel] ([idTabel])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[tabelPerson] CHECK CONSTRAINT [FK_days_tabel]

--================================================================================================================================================================
-- Типы дней табеля
--================================================================================================================================================================

CREATE TABLE [dbo].[typeDay](
	[idTypeDay] [int] NOT NULL,
	[t_name] [varchar](3) NULL,
	[t_desc] [varchar](200) NULL,
 CONSTRAINT [PK_typeDay] PRIMARY KEY CLUSTERED 
(
	[idTypeDay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (0, '','Пусто')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (1, 'Я','Явка (в случае явки записывают количество отработанных часов)')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (2, 'В','Выходной')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (3, 'К','Командировка')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (4, 'ОТ','Работа в выходной день по приказу')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (5, 'РВ','Работа в выходной день по приказу')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (6, 'ДО','Отпуск без сохранения зарплаты, предоставленый по разрешению работодателя')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (7, 'Б','Временная нетрудоспособность с назначением пособия')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (8, 'У','Дополнительный отпуск в связи с обучением с сохранением среднего заработка')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (9, 'Т','Временная нетрудоспособность без назначения пособия')
INSERT INTO [dbo].[typeDay] ([idTypeDay],[t_name] ,[t_desc])  VALUES (10,'Д','Дистанционная работа')


--================================================================================================================================================================
-- Дни табеля
--================================================================================================================================================================
CREATE TABLE [dbo].[TabelDay](
	[td_Id] [int] IDENTITY(1,1) NOT NULL,
	[td_TabelPersonId] [int] NOT NULL,
	[td_Day] [int] NOT NULL,
	[td_KindId] [int] NOT NULL,
	[td_Hours] [numeric](18, 1) NULL,
	[td_Hours2] [numeric](18, 1) NULL,
 CONSTRAINT [PK_TabelDay] PRIMARY KEY CLUSTERED 
(
	[td_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[TabelDay]  WITH CHECK ADD  CONSTRAINT [FK_TabelDay_tabelPerson] FOREIGN KEY([td_TabelPersonId])
REFERENCES [dbo].[tabelPerson] ([tp_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[TabelDay] CHECK CONSTRAINT [FK_TabelDay_tabelPerson]

ALTER TABLE [dbo].[TabelDay]  WITH CHECK ADD  CONSTRAINT [FK_TabelDay_typeDay] FOREIGN KEY([td_KindId])
REFERENCES [dbo].[typeDay] ([idTypeDay])

ALTER TABLE [dbo].[TabelDay] CHECK CONSTRAINT [FK_TabelDay_typeDay]

--================================================================================================================================================================
-- Задачи 
--================================================================================================================================================================
CREATE TABLE [dbo].[TargetTask](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tt_ModPersonId] [int] NOT NULL,
	[tt_name] [varchar](150) NULL,
	[tt_proc_task] [numeric](18, 2) NULL,
	[tt_proc_fact] [numeric](18, 2) NULL,
	[tt_AttachFile] [varchar](max) NULL,
	[tt_idFile] [int] NULL,
 CONSTRAINT [PK_TargetTask_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[TargetTask]  WITH CHECK ADD  CONSTRAINT [FK_TargetTask_ModPerson] FOREIGN KEY([tt_ModPersonId])
REFERENCES [dbo].[ModPerson] ([md_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[TargetTask] CHECK CONSTRAINT [FK_TargetTask_ModPerson]

--================================================================================================================================================================
-- Транспорт
--================================================================================================================================================================
CREATE TABLE [dbo].[Transport](
	[tr_Id] [int] IDENTITY(1,1) NOT NULL,
	[tr_Number] [varchar](20) NULL,
	[tr_DateCreate] [datetime] NOT NULL,
	[tr_Month] [int] NOT NULL,
	[tr_Year] [int] NOT NULL,
	[tr_UserId] [int] NOT NULL,
	[tr_OtdelId] [int] NOT NULL,
	[tr_IsClosed] [bit] NULL,
 CONSTRAINT [PK_Transport] PRIMARY KEY CLUSTERED 
(
	[tr_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Transport]  WITH CHECK ADD  CONSTRAINT [FK_Transport_otdels] FOREIGN KEY([tr_OtdelId])
REFERENCES [dbo].[otdels] ([idOtdel])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[Transport] CHECK CONSTRAINT [FK_Transport_otdels]

ALTER TABLE [dbo].[Transport]  WITH CHECK ADD  CONSTRAINT [FK_Transport_users] FOREIGN KEY([tr_UserId])
REFERENCES [dbo].[users] ([idUser])

ALTER TABLE [dbo].[Transport] CHECK CONSTRAINT [FK_Transport_users]

--================================================================================================================================================================
-- Персонал транспорта
--================================================================================================================================================================

CREATE TABLE [dbo].[TransPerson](
	[tp_Id] [int] IDENTITY(1,1) NOT NULL,
	[tp_PersonId] [int] NOT NULL,
	[tp_TranspId] [int] NOT NULL,
	[tp_tarif] [numeric](18, 2) NULL,
	[tp_Kompens] [numeric](18, 2) NULL,
 CONSTRAINT [PK_TransPerson] PRIMARY KEY CLUSTERED 
(
	[tp_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[TransPerson]  WITH CHECK ADD  CONSTRAINT [FK_TransPerson_personal] FOREIGN KEY([tp_PersonId])
REFERENCES [dbo].[personal] ([idPerson])

ALTER TABLE [dbo].[TransPerson] CHECK CONSTRAINT [FK_TransPerson_personal]

ALTER TABLE [dbo].[TransPerson]  WITH CHECK ADD  CONSTRAINT [FK_TransPerson_Transport] FOREIGN KEY([tp_TranspId])
REFERENCES [dbo].[Transport] ([tr_Id])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[TransPerson] CHECK CONSTRAINT [FK_TransPerson_Transport]

--================================================================================================================================================================
-- Дни транспорта
--================================================================================================================================================================

CREATE TABLE [dbo].[TransDay](
	[td_Id] [int] IDENTITY(1,1) NOT NULL,
	[td_TransPersonId] [int] NOT NULL,
	[td_Day] [int] NOT NULL,
	[td_Kind] [int] NULL,
 CONSTRAINT [PK_TransDay] PRIMARY KEY CLUSTERED 
(
	[td_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TransDay]  WITH CHECK ADD  CONSTRAINT [FK_TransDay_TransPerson] FOREIGN KEY([td_TransPersonId])
REFERENCES [dbo].[TransPerson] ([tp_Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TransDay] CHECK CONSTRAINT [FK_TransDay_TransPerson]



--================================================================================================================================================================
-- 
--================================================================================================================================================================

CREATE TABLE [dbo].[UserOtdels](
	[Otdel_ID] [int] NOT NULL,
	[User_ID] [int] NOT NULL
) ON [PRIMARY]

ALTER TABLE [dbo].[UserOtdels]  WITH CHECK ADD  CONSTRAINT [FK_UserOtdels_otdels] FOREIGN KEY([Otdel_ID])
REFERENCES [dbo].[otdels] ([idOtdel])

ALTER TABLE [dbo].[UserOtdels] CHECK CONSTRAINT [FK_UserOtdels_otdels]

ALTER TABLE [dbo].[UserOtdels]  WITH CHECK ADD  CONSTRAINT [FK_UserOtdels_users] FOREIGN KEY([User_ID])
REFERENCES [dbo].[users] ([idUser])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[UserOtdels] CHECK CONSTRAINT [FK_UserOtdels_users]
