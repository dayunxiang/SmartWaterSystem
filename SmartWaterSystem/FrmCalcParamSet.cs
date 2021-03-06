﻿using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Common;

namespace SmartWaterSystem
{
	public partial class FrmCalcParamSet : DevExpress.XtraEditors.XtraForm
	{
		public FrmCalcParamSet()
		{
			InitializeComponent();

            cbArith.Items.Add("直接平均法");
            cbArith.Items.Add("去最值平均法");
		}

		private void btnSave_Click(object sender, EventArgs e)
        {
            #region controls validation
            if (string.IsNullOrEmpty(txtMax1.Text) || Convert.ToInt32(txtMax1.Text) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("低频段傅里叶数据区间上限不能为空或小于零!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMax1.Focus();
                txtMax1.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtMin1.Text) || Convert.ToInt32(txtMin1.Text) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("低频段傅里叶数据区间下限不能为空或小于零!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMin1.Focus();
                txtMin1.SelectAll();
                return;
            }

            if (Convert.ToInt32(txtMax1.Text) <= Convert.ToInt32(txtMin1.Text))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("低频段傅里叶数据区间下限不能大于或等于上限!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMax1.Focus();
                txtMax1.SelectAll();
                return;
            }

            if (string.IsNullOrEmpty(txtMax2.Text) || Convert.ToInt32(txtMax2.Text) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("高频段傅里叶数据区间上限不能为空或小于零!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMax2.Focus();
                txtMax2.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtMin2.Text) || Convert.ToInt32(txtMin2.Text) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("高频段傅里叶数据区间下限不能为空或小于零!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMin2.Focus();
                txtMin2.SelectAll();
                return;
            }

            if (Convert.ToInt32(txtMax2.Text) <= Convert.ToInt32(txtMin2.Text))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("高频段傅里叶数据区间下限不能大于或等于上限!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMax2.Focus();
                txtMax2.SelectAll();
                return;
            }

            if (string.IsNullOrEmpty(txtStandardAMP.Text) || Convert.ToDecimal(txtStandardAMP.Text) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("请输入有效的静态漏水标准幅度值!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtStandardAMP.Focus();
                txtStandardAMP.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtDCCompLen.Text) || Convert.ToDecimal(txtDCCompLen.Text) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("请输入有效的直流分量值!", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDCCompLen.Focus();
                txtDCCompLen.SelectAll();
                return;
            }

            #endregion
            Settings.Instance.SetValue(SettingKeys.Max1, txtMax1.Text);
			Settings.Instance.SetValue(SettingKeys.Max2, txtMax2.Text);
			Settings.Instance.SetValue(SettingKeys.Min1, txtMin1.Text);
            Settings.Instance.SetValue(SettingKeys.Min2, txtMin2.Text);
			Settings.Instance.SetValue(SettingKeys.LeakHZ_Template, txtLeakHZ.Text);
            Settings.Instance.SetValue(SettingKeys.MaxStandardAMP, txtStandardAMP.Text);
            Settings.Instance.SetValue(SettingKeys.DCComponentLen, txtDCCompLen.Text);

			if(cbArith.SelectedIndex==0)
                Settings.Instance.SetValue(SettingKeys.Calc, (1).ToString());
			else if (cbArith.SelectedIndex == 1)
                Settings.Instance.SetValue(SettingKeys.Calc, (2).ToString());

            XtraMessageBox.Show("保存成功！", GlobalValue.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			this.Close();
		}

		private void FrmCalcParamSet_Load(object sender, EventArgs e)
		{
			txtMax1.Text = Settings.Instance.GetString(SettingKeys.Max1);
			txtMax2.Text = Settings.Instance.GetString(SettingKeys.Max2);
			txtMin1.Text = Settings.Instance.GetString(SettingKeys.Min1);
			txtMin2.Text = Settings.Instance.GetString(SettingKeys.Min2);
			txtLeakHZ.Text = Settings.Instance.GetString(SettingKeys.LeakHZ_Template);
            txtStandardAMP.Text = Settings.Instance.GetString(SettingKeys.MaxStandardAMP);
            txtDCCompLen.Text = Settings.Instance.GetString(SettingKeys.DCComponentLen);

			if (Settings.Instance.GetString(SettingKeys.Calc) == (1).ToString())
				cbArith.SelectedIndex = 0;
			else if (Settings.Instance.GetString(SettingKeys.Calc) == (2).ToString())
				cbArith.SelectedIndex = 1;
		}

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
	}
}
