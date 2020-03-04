using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicResearchWebApi.Models
{
    public class AzureSongModel
    {
        public Decimal Song_id { get; set; }

        public String Song_name { get; set; }

        public Decimal Song_length { get; set; }

        public Decimal Tempo { get; set; }

        public Decimal Total_beats { get; set; }

        public Decimal Average_beats { get; set; }

        public Decimal Zcr_mean { get; set; }

        public Decimal Zcr_var { get; set; }

        public Decimal Zcr_std { get; set; }

        public Decimal Cent_mean { get; set; }

        public Decimal Cent_var { get; set; }

        public Decimal Cent_std { get; set; }

        public Decimal Rolloff_mean { get; set; }

        public Decimal Rolloff_var { get; set; }

        public Decimal Rolloff_std { get; set; }

        public Decimal Chroma_mean { get; set; }

        public Decimal Chroma_var { get; set; }

        public Decimal Chroma_std { get; set; }

        public Decimal Chroma_cqt_mean { get; set; }

        public Decimal Chroma_cqt_var { get; set; }

        public Decimal Chroma_cqt_std { get; set; }

        public Decimal Chroma_cens_mean { get; set; }

        public Decimal Chroma_cens_var { get; set; }

        public Decimal Chroma_cens_std { get; set; }

        public Decimal Mfccs_mean { get; set; }

        public Decimal Mfccs_var { get; set; }

        public Decimal Mfccs_std { get; set; }

        public Decimal Mfcc_delta_mean { get; set; }

        public Decimal Mfcc_delta_var { get; set; }

        public Decimal Mfcc_delta_std { get; set; }

        public Decimal Mel_mean { get; set; }

        public Decimal Mel_var { get; set; }

        public Decimal Mel_std { get; set; }

        public Decimal Tonnetz_mean { get; set; }

        public Decimal Tonnetz_var { get; set; }

        public Decimal Tonnetz_std { get; set; }

        public Decimal Spec_bw_mean { get; set; }

        public Decimal Spec_bw_var { get; set; }

        public Decimal Spec_bw_std { get; set; }

        public Decimal Spec_con_mean { get; set; }

        public Decimal Spec_con_var { get; set; }

        public Decimal Spec_con_std { get; set; }

        public Decimal Harmonic_mean { get; set; }

        public Decimal Harmonic_var { get; set; }

        public Decimal Harmonic_std { get; set; }

        public Decimal Percussive_mean { get; set; }

        public Decimal Percussive_var { get; set; }

        public Decimal Percussive_std { get; set; }

        public String Genre { get; set; }
    }
}